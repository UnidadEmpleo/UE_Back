using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Application.Clinica.Farmacia.Commands;
using API.Domain.Clinica;
using API.Domain.Clinica.Inventario;
using Microsoft.EntityFrameworkCore;
using API.Tests.Clinica.Inventario.TestHelpers;
using Xunit;

namespace API.Tests.Clinica.Inventario
{
    public class EntradasTests : InventoryTestBase
    {
        [Fact]
        public async Task Entrada_Confirmar_AumentaExistencias_Y_EstadoConfirmado()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Paracetamol", Existencias = 10, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateEntradaMedicamento.Handler(factory, mapper).Handle(
                new CreateEntradaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-001",
                    TipoDocumentoId = 1,
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 5, PrecioUnitarioConIVA = 2.3m }
                    }
                }, CancellationToken.None);

            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarEntrada.Handler(factory).Handle(
                new ConfirmarEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);

            Assert.True(confirmar.IsSuccess);
            Assert.Equal(EstadoMovimiento.Entregado, confirmar.Value.Estado);

            var med = await db.Medicamentos.FindAsync(1);
            Assert.NotNull(med);
            Assert.Equal(15, med!.Existencias);
        }

        [Fact]
        public async Task Entrada_Cancelar_Confirmada_RevierteExistencias_Y_EstadoCancelado()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Ibuprofeno", Existencias = 20, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateEntradaMedicamento.Handler(factory, mapper).Handle(
                new CreateEntradaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-002",
                    TipoDocumentoId = 1,
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 7 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarEntrada.Handler(factory).Handle(
                new ConfirmarEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(confirmar.IsSuccess);

            var cancelar = await new CancelarEntrada.Handler(factory).Handle(
                new CancelarEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(cancelar.IsSuccess);
            Assert.Equal(EstadoMovimiento.Cancelado, cancelar.Value.Estado);

            var med = await db.Medicamentos.FindAsync(1);
            Assert.NotNull(med);
            Assert.Equal(20, med!.Existencias);
        }

        [Fact]
        public async Task Entrada_Update_Borrador_Modifica_Detalles()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.AddRange(
                new Medicamento
                {
                    Id = 1, SustanciaActiva = "Losartan", Existencias = 5, Activo = true,
                    FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
                },
                new Medicamento
                {
                    Id = 2, SustanciaActiva = "Aspirina", Existencias = 10, Activo = true,
                    FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
                });
            await db.SaveChangesAsync();

            var crear = await new CreateEntradaMedicamento.Handler(factory, mapper).Handle(
                new CreateEntradaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-003",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 2 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var update = await new UpdateEntradaMedicamento.Handler(factory, mapper).Handle(
                new UpdateEntradaMedicamento.Command
                {
                    Id = crear.Value.Id,
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-003-UPD",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 2, Cantidad = 9 }
                    }
                }, CancellationToken.None);

            Assert.True(update.IsSuccess);

            var entradaDb = await db.EntradasMedicamentos
                .Include(e => e.Detalles)
                .FirstOrDefaultAsync(e => e.Id == crear.Value.Id);
            Assert.NotNull(entradaDb);
            Assert.Single(entradaDb!.Detalles);
            Assert.Equal(2, entradaDb.Detalles.First().MedicamentoId);
            Assert.Equal(9, entradaDb.Detalles.First().Cantidad);
            Assert.Equal("E-003-UPD", entradaDb.NumeroDocumento);
        }

        [Fact]
        public async Task Entrada_Delete_Borrador_Elimina()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Clorfenamina", Existencias = 3, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateEntradaMedicamento.Handler(factory, mapper).Handle(
                new CreateEntradaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-004",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 1 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var delete = await new DeleteEntrada.Handler(factory).Handle(
                new DeleteEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);

            Assert.True(delete.IsSuccess);

            var exists = await db.EntradasMedicamentos.AnyAsync(e => e.Id == crear.Value.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task Entrada_Delete_Confirmada_Falla()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Omeprazol", Existencias = 12, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateEntradaMedicamento.Handler(factory, mapper).Handle(
                new CreateEntradaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "E-005",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateEntradaMedicamento.DetalleEntradaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 2 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarEntrada.Handler(factory).Handle(
                new ConfirmarEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(confirmar.IsSuccess);

            var delete = await new DeleteEntrada.Handler(factory).Handle(
                new DeleteEntrada.Command { Id = crear.Value.Id }, CancellationToken.None);

            Assert.False(delete.IsSuccess);
            Assert.Contains("No se puede eliminar una entrada confirmada", delete.Error ?? string.Empty);
        }
    }
}
