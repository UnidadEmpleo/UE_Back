using API.Application.Reportes.Queries;
using API.Domain.Clinica;
using API.Persistence;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace API.Tests.Reportes.Queries
{
    public class ReporteDiarioAtencionesMedicasQueryTests : IDisposable
    {
        private readonly CorporacionDbContext _corporacionContext;
        private readonly AppDbContext _appContext;
        private readonly Mock<ICorporacionDbContextFactory> _factoryMock;
        private readonly Mock<IHttpContextAccessor> _httpContextMock;
        private readonly IMapper _mapper;

        public ReporteDiarioAtencionesMedicasQueryTests()
        {
            // Setup In-Memory Database
            var corporacionOptions = new DbContextOptionsBuilder<CorporacionDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var appOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _corporacionContext = new CorporacionDbContext(corporacionOptions, null);
            //_appContext = new AppDbContext(appOptions, null);

            // Setup Factory Mock
            _factoryMock = new Mock<ICorporacionDbContextFactory>();
            _factoryMock.Setup(f => f.CreateAsync())
                .ReturnsAsync(_corporacionContext);

            // Setup HttpContext Mock
            _httpContextMock = new Mock<IHttpContextAccessor>();

            // Setup AutoMapper
            //var configuration = new MapperConfiguration(cfg =>
            //{
            //    cfg.AddProfile<Application.Core.MappingProfiles>();
            //});
            //_mapper = configuration.CreateMapper();
        }

        [Fact]
        public async Task Handle_ShouldReturnCorrectReportData_WhenAtencionesMedicasExist()
        {
            // Arrange
            var fecha = new DateTime(2025, 7, 25);
            var clinicaId = 1;

            // Seed test data
            var clinica = new ClinicaMedica 
            { 
                Id = clinicaId, 
                Nombre = "Santa Rosa",
                CorporacionId = 1
            };

            var paciente = new Paciente
            {
                Id = 1,
                Nombre = "Alberto",
                PrimerApellido = "Gutierrez",
                SegundoApellido = "Gonzalez",
                CorporacionId = 1
            };

            var atenciones = new List<AtencionMedica>
            {
                new AtencionMedica
                {
                    Id = 1,
                    Inicio = fecha.AddHours(7),
                    Fin = fecha.AddHours(7.5),
                    ClinicaId = clinicaId,
                    PacienteId = 1,
                    MedicoId = "medico-123",
                    Estado = EstadoAtencion.Programada,
                    CorporacionId = 1
                },
                new AtencionMedica
                {
                    Id = 2,
                    Inicio = fecha.AddHours(8),
                    Fin = fecha.AddHours(8.5),
                    ClinicaId = clinicaId,
                    PacienteId = 1,
                    MedicoId = "medico-123",
                    Estado = EstadoAtencion.EnEspera,
                    CorporacionId = 1
                }
            };

            await _corporacionContext.Set<ClinicaMedica>().AddAsync(clinica);
            await _corporacionContext.Set<Paciente>().AddAsync(paciente);
            await _corporacionContext.Set<AtencionMedica>().AddRangeAsync(atenciones);
            await _corporacionContext.SaveChangesAsync();

            var usuario = new Domain.Seguridad.Usuario
            {
                Id = "user-123",
                Nombre = "Sergio",
                PrimerApellido = "Silva",
                SegundoApellido = "Test"
            };
            await _appContext.Usuarios.AddAsync(usuario);
            await _appContext.SaveChangesAsync();

            var query = new GetReporteDiarioAtencionesMedicas.Query
            {
                FechaInicio = fecha,
                ClinicaId = clinicaId
            };

            var handler = new GetReporteDiarioAtencionesMedicas.Handler(
                _factoryMock.Object,
                _appContext,
                _mapper,
                _httpContextMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.TotalAtenciones.Should().Be(2);
            result.Value.ClinicaNombre.Should().Be("Santa Rosa");
            result.Value.Atenciones.Should().HaveCount(2);
            result.Value.Atenciones[0].Numero.Should().Be(1);
            result.Value.Atenciones[0].PacienteNombre.Should().Contain("Gutierrez");
        }

        [Fact]
        public async Task Handle_ShouldFilterByDateRange_Correctly()
        {
            // Arrange
            var fechaInicio = new DateTime(2025, 7, 25);
            var fechaFin = new DateTime(2025, 7, 26);

            var atenciones = new List<AtencionMedica>
            {
                new AtencionMedica
                {
                    Id = 1,
                    Inicio = fechaInicio.AddHours(10),
                    Fin = fechaInicio.AddHours(11),
                    ClinicaId = 1,
                    MedicoId = "medico-123",
                    CorporacionId = 1
                },
                new AtencionMedica
                {
                    Id = 2,
                    Inicio = fechaFin.AddHours(10),
                    Fin = fechaFin.AddHours(11),
                    ClinicaId = 1,
                    MedicoId = "medico-123",
                    CorporacionId = 1
                },
                new AtencionMedica
                {
                    Id = 3,
                    Inicio = fechaFin.AddDays(2), // Fuera del rango
                    Fin = fechaFin.AddDays(2).AddHours(1),
                    ClinicaId = 1,
                    MedicoId = "medico-123",
                    CorporacionId = 1
                }
            };

            await _corporacionContext.Set<AtencionMedica>().AddRangeAsync(atenciones);
            await _corporacionContext.SaveChangesAsync();

            var query = new GetReporteDiarioAtencionesMedicas.Query
            {
                FechaInicio = fechaInicio,
                FechaFin = fechaFin
            };

            var handler = new GetReporteDiarioAtencionesMedicas.Handler(
                _factoryMock.Object,
                _appContext,
                _mapper,
                _httpContextMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.TotalAtenciones.Should().Be(2);
        }

        [Fact]
        public async Task Handle_ShouldReturnEmptyList_WhenNoAtencionesMedicasExist()
        {
            // Arrange
            var query = new GetReporteDiarioAtencionesMedicas.Query
            {
                FechaInicio = DateTime.Today
            };

            var handler = new GetReporteDiarioAtencionesMedicas.Handler(
                _factoryMock.Object,
                _appContext,
                _mapper,
                _httpContextMock.Object
            );

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.TotalAtenciones.Should().Be(0);
            result.Value.Atenciones.Should().BeEmpty();
        }

        public void Dispose()
        {
            _corporacionContext.Dispose();
            _appContext.Dispose();
        }
    }
}