using API.Application.Reportes.Services.CSV;
using API.DTOs.Reportes;
using FluentAssertions;
using System.Text;
using Xunit;

namespace API.Tests.Reportes.Services.CSV
{
    public class ReporteDiarioCSVGeneratorTests
    {
        [Fact]
        public async Task GenerateAsync_ShouldReturnValidCSVBytes_WhenDataIsProvided()
        {
            // Arrange
            var generator = new ReporteDiarioCSVGenerator();
            var data = CreateSampleReportData();

            // Act
            var result = await generator.GenerateAsync(data);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();

            var csvContent = Encoding.UTF8.GetString(result);
            csvContent.Should().Contain("Reporte diario de atenciones médicas");
            csvContent.Should().Contain("Santa Rosa");
            csvContent.Should().Contain("Gutierrez Gonzalez Alberto");
        }

        [Fact]
        public async Task GenerateAsync_ShouldIncludeHeaderRow()
        {
            // Arrange
            var generator = new ReporteDiarioCSVGenerator();
            var data = CreateSampleReportData();

            // Act
            var result = await generator.GenerateAsync(data);
            var csvContent = Encoding.UTF8.GetString(result);
            var lines = csvContent.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            // Assert
            lines.Should().Contain(line => 
                line.Contains("Numero") || 
                line.Contains("Hora") || 
                line.Contains("PacienteNombre"));
        }

        [Fact]
        public async Task GenerateAsync_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var generator = new ReporteDiarioCSVGenerator();
            var data = new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = DateTime.Today,
                ClinicaNombre = "Clínica \"Santa Rosa\"",
                ImpresoPor = "Test User",
                TotalAtenciones = 1,
                Atenciones = new List<AtencionReporteItemDTO>
                {
                    new AtencionReporteItemDTO
                    {
                        Numero = 1,
                        Hora = TimeSpan.FromHours(7),
                        PacienteNombre = "García, José \"Pepe\"",
                        MedicoNombre = "Dr. López",
                        Status = "Programada",
                        Tipo = "Consulta",
                        Especialidad = "NA",
                        Asigno = "Sistema"
                    }
                }
            };

            // Act
            var result = await generator.GenerateAsync(data);

            // Assert
            result.Should().NotBeNull();
            var csvContent = Encoding.UTF8.GetString(result);
            csvContent.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void GetContentType_ShouldReturnCSVMimeType()
        {
            // Arrange
            var generator = new ReporteDiarioCSVGenerator();

            // Act
            var contentType = generator.GetContentType();

            // Assert
            contentType.Should().Be("text/csv");
        }

        [Fact]
        public void GetFileExtension_ShouldReturnCSVExtension()
        {
            // Arrange
            var generator = new ReporteDiarioCSVGenerator();

            // Act
            var extension = generator.GetFileExtension();

            // Assert
            extension.Should().Be(".csv");
        }

        private static ReporteDiarioAtencionesMedicasDTO CreateSampleReportData()
        {
            return new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = new DateTime(2025, 7, 25),
                ClinicaNombre = "Santa Rosa",
                ImpresoPor = "Sergio Silva",
                TotalAtenciones = 2,
                Atenciones = new List<AtencionReporteItemDTO>
                {
                    new AtencionReporteItemDTO
                    {
                        Numero = 1,
                        Hora = new TimeSpan(7, 0, 0),
                        PacienteNombre = "Gutierrez Gonzalez Alberto",
                        MedicoNombre = "Perez Elias Alejandro",
                        Status = "Programada",
                        Tipo = "Especialidad",
                        Especialidad = "Ginecología",
                        Asigno = "Sistema"
                    }
                }
            };
        }
    }
}