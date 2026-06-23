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
    public class SalidasTests : InventoryTestBase
    {
        [Fact]
        public async Task Salida_Crear_InsuficienteExistencia_Falla()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Amoxicilina", Existencias = 3, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var result = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-001",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 5 }
                    }
                }, CancellationToken.None);

            Assert.False(result.IsSuccess);
            Assert.Contains("Existencias insuficientes", result.Error ?? string.Empty);
        }

        [Fact]
        public async Task Salida_Confirmar_DisminuyeExistencias_Y_EstadoConfirmado()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Diclofenaco", Existencias = 10, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-002",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 4 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarSalida.Handler(factory).Handle(
                new ConfirmarSalida.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(confirmar.IsSuccess);
            Assert.Equal(EstadoMovimiento.Entregado, confirmar.Value.Estado);

            var med = await db.Medicamentos.FindAsync(1);
            Assert.NotNull(med);
            Assert.Equal(6, med!.Existencias);
        }

        [Fact]
        public async Task Salida_Cancelar_Confirmada_RevierteExistencias_Y_EstadoCancelado()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Aciclovir", Existencias = 15, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-003",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 5 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarSalida.Handler(factory).Handle(
                new ConfirmarSalida.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(confirmar.IsSuccess);

            var cancelar = await new CancelarSalida.Handler(factory).Handle(
                new CancelarSalida.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(cancelar.IsSuccess);
            Assert.Equal(EstadoMovimiento.Cancelado, cancelar.Value.Estado);

            var med = await db.Medicamentos.FindAsync(1);
            Assert.NotNull(med);
            Assert.Equal(15, med!.Existencias);
        }

        [Fact]
        public async Task Salida_Update_Borrador_Modifica_Detalles()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.AddRange(
                new Medicamento
                {
                    Id = 1, SustanciaActiva = "Metformina", Existencias = 30, Activo = true,
                    FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
                },
                new Medicamento
                {
                    Id = 2, SustanciaActiva = "Insulina", Existencias = 50, Activo = true,
                    FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
                });
            await db.SaveChangesAsync();

            var crear = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-004",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 3 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var update = await new UpdateSalidaMedicamento.Handler(factory, mapper).Handle(
                new UpdateSalidaMedicamento.Command
                {
                    Id = crear.Value.Id,
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-004-UPD",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 2, Cantidad = 4 }
                    }
                }, CancellationToken.None);

            Assert.True(update.IsSuccess);

            var salidaDb = await db.SalidasMedicamentos
                .Include(s => s.Detalles)
                .FirstOrDefaultAsync(s => s.Id == crear.Value.Id);
            Assert.NotNull(salidaDb);
            Assert.Single(salidaDb!.Detalles);
            Assert.Equal(2, salidaDb.Detalles.First().MedicamentoId);
            Assert.Equal(4, salidaDb.Detalles.First().Cantidad);
            Assert.Equal("S-004-UPD", salidaDb.NumeroDocumento);
        }

        [Fact]
        public async Task Salida_Delete_Borrador_Elimina()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Ketorolaco", Existencias = 8, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-005",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 2 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var delete = await new DeleteSalida.Handler(factory).Handle(
                new DeleteSalida.Command { Id = crear.Value.Id }, CancellationToken.None);

            Assert.True(delete.IsSuccess);

            var exists = await db.SalidasMedicamentos.AnyAsync(s => s.Id == crear.Value.Id);
            Assert.False(exists);
        }

        [Fact]
        public async Task Salida_Delete_Confirmada_Falla()
        {
            var db = CreateInMemoryContext(Guid.NewGuid().ToString());
            await SeedBaseCatalogsAsync(db);
            var mapper = CreateMapper();
            var factory = new FakeCorporacionDbContextFactory(db);

            db.Medicamentos.Add(new Medicamento
            {
                Id = 1, SustanciaActiva = "Azitromicina", Existencias = 11, Activo = true,
                FechaCreacion = DateTime.UtcNow, FechaUltimaActualizacion = DateTime.UtcNow, FarmaciaId = 1
            });
            await db.SaveChangesAsync();

            var crear = await new CreateSalidaMedicamento.Handler(factory, mapper).Handle(
                new CreateSalidaMedicamento.Command
                {
                    CorporacionId = "CORP",
                    FarmaciaId = 1,
                    Ejercicio = DateTime.UtcNow.Year,
                    NumeroDocumento = "S-006",
                    FechaMovimiento = DateTime.UtcNow,
                    Detalles = new List<CreateSalidaMedicamento.DetalleSalidaCommand>
                    {
                        new() { MedicamentoId = 1, Cantidad = 1 }
                    }
                }, CancellationToken.None);
            Assert.True(crear.IsSuccess);

            var confirmar = await new ConfirmarSalida.Handler(factory).Handle(
                new ConfirmarSalida.Command { Id = crear.Value.Id }, CancellationToken.None);
            Assert.True(confirmar.IsSuccess);

            var delete = await new DeleteSalida.Handler(factory).Handle(
                new DeleteSalida.Command { Id = crear.Value.Id }, CancellationToken.None);

            Assert.False(delete.IsSuccess);
            Assert.Contains("No se puede eliminar una salida confirmada", delete.Error ?? string.Empty);
        }
    }
}
