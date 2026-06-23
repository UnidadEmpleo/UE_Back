using API.Application.Reportes.Commands;
using API.Application.Reportes.Queries;
using API.DTOs.Reportes;
using API.Seguridad.Application.Core;
using FluentAssertions;
using MediatR;
using Moq;
using Xunit;

namespace API.Tests.Reportes.Commands
{
    public class ExportarReporteDiarioAtencionesCommandTests
    {
        private readonly Mock<IMediator> _mediatorMock;

        public ExportarReporteDiarioAtencionesCommandTests()
        {
            _mediatorMock = new Mock<IMediator>();
        }

        [Fact]
        public async Task Handle_ShouldReturnPDFFile_WhenFormatIsPDF()
        {
            // Arrange
            var reportData = CreateSampleReportData();
            var queryResult = Result<ReporteDiarioAtencionesMedicasDTO>.Success(reportData);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetReporteDiarioAtencionesMedicas.Query>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var command = new ExportarReporteDiarioAtenciones.Command
            {
                FechaInicio = DateTime.Today,
                Formato = TiposReporte.PDF
            };

            var handler = new ExportarReporteDiarioAtenciones.Handler(_mediatorMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ContentType.Should().Be("application/pdf");
            result.Value.FileName.Should().EndWith(".pdf");
            result.Value.FileBytes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnCSVFile_WhenFormatIsCSV()
        {
            // Arrange
            var reportData = CreateSampleReportData();
            var queryResult = Result<ReporteDiarioAtencionesMedicasDTO>.Success(reportData);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetReporteDiarioAtencionesMedicas.Query>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var command = new ExportarReporteDiarioAtenciones.Command
            {
                FechaInicio = DateTime.Today,
                Formato = TiposReporte.CSV
            };

            var handler = new ExportarReporteDiarioAtenciones.Handler(_mediatorMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.ContentType.Should().Be("text/csv");
            result.Value.FileName.Should().EndWith(".csv");
            result.Value.FileBytes.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Handle_ShouldReturnFailure_WhenQueryFails()
        {
            // Arrange
            var queryResult = Result<ReporteDiarioAtencionesMedicasDTO>.Failure("Error al obtener datos", 400);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetReporteDiarioAtencionesMedicas.Query>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var command = new ExportarReporteDiarioAtenciones.Command
            {
                FechaInicio = DateTime.Today,
                Formato = TiposReporte.PDF
            };

            var handler = new ExportarReporteDiarioAtenciones.Handler(_mediatorMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("No se pudo obtener los datos del reporte");
        }

        [Fact]
        public async Task Handle_ShouldGenerateCorrectFileName()
        {
            // Arrange
            var fecha = new DateTime(2025, 7, 25);
            var reportData = CreateSampleReportData();
            var queryResult = Result<ReporteDiarioAtencionesMedicasDTO>.Success(reportData);

            _mediatorMock
                .Setup(m => m.Send(It.IsAny<GetReporteDiarioAtencionesMedicas.Query>(), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(queryResult);

            var command = new ExportarReporteDiarioAtenciones.Command
            {
                FechaInicio = fecha,
                Formato = TiposReporte.PDF
            };

            var handler = new ExportarReporteDiarioAtenciones.Handler(_mediatorMock.Object);

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            result.Value.FileName.Should().Be("ReporteAtenciones_20250725.pdf");
        }

        private static ReporteDiarioAtencionesMedicasDTO CreateSampleReportData()
        {
            return new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = DateTime.Today,
                ClinicaNombre = "Test Clinic",
                ImpresoPor = "Test User",
                TotalAtenciones = 1,
                Atenciones = new List<AtencionReporteItemDTO>
                {
                    new AtencionReporteItemDTO
                    {
                        Numero = 1,
                        Hora = TimeSpan.FromHours(7),
                        PacienteNombre = "Test Patient",
                        MedicoNombre = "Test Doctor",
                        Status = "Programada",
                        Tipo = "Consulta",
                        Especialidad = "NA",
                        Asigno = "Sistema"
                    }
                }
            };
        }
    }
}