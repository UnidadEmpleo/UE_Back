using API.Application.Reportes.Services.PDF;
using API.DTOs.Reportes;
using FluentAssertions;
using Xunit;

namespace API.Tests.Reportes.Services.PDF
{
    public class ReporteDiarioPDFGeneratorTests
    {
        [Fact]
        public async Task GenerateAsync_ShouldReturnValidPDFBytes_WhenDataIsProvided()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();
            var data = CreateSampleReportData();

            // Act
            var result = await generator.GenerateAsync(data);

            // Assert
            result.Should().NotBeNull();
            result.Should().NotBeEmpty();
            result.Length.Should().BeGreaterThan(0);
            
            // PDF files start with %PDF-
            var pdfHeader = System.Text.Encoding.ASCII.GetString(result.Take(4).ToArray());
            pdfHeader.Should().Be("%PDF");
        }

        [Fact]
        public async Task GenerateAsync_ShouldHandleEmptyAtencionesList()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();
            var data = new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = DateTime.Today,
                ClinicaNombre = "Test Clinic",
                ImpresoPor = "Test User",
                TotalAtenciones = 0,
                Atenciones = new List<AtencionReporteItemDTO>()
            };

            // Act
            var result = await generator.GenerateAsync(data);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GenerateAsync_ShouldHandleLargeDataSet()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();
            var data = CreateLargeReportData(1000); // 1000 atenciones

            // Act
            var result = await generator.GenerateAsync(data);

            // Assert
            result.Should().NotBeNull();
            result.Length.Should().BeGreaterThan(0);
        }

        [Fact]
        public void GetContentType_ShouldReturnPDFMimeType()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();

            // Act
            var contentType = generator.GetContentType();

            // Assert
            contentType.Should().Be("application/pdf");
        }

        [Fact]
        public void GetFileExtension_ShouldReturnPDFExtension()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();

            // Act
            var extension = generator.GetFileExtension();

            // Assert
            extension.Should().Be(".pdf");
        }

        private static ReporteDiarioAtencionesMedicasDTO CreateSampleReportData()
        {
            return new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = new DateTime(2025, 7, 25),
                ClinicaNombre = "Santa Rosa",
                ImpresoPor = "Sergio Silva",
                TotalAtenciones = 3,
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
                        Especialidad = "Ginecología y Obstetricia",
                        Asigno = "Fernandez Garcia Marisol"
                    },
                    new AtencionReporteItemDTO
                    {
                        Numero = 2,
                        Hora = new TimeSpan(7, 20, 0),
                        PacienteNombre = "Salazar Peñaloza José Manuel",
                        MedicoNombre = "Perez Elias Alejandro",
                        Status = "En Espera",
                        Tipo = "Especialidad",
                        Especialidad = "Ginecología y Obstetricia",
                        Asigno = "Fernandez Garcia Marisol"
                    }
                }
            };
        }

        private static ReporteDiarioAtencionesMedicasDTO CreateLargeReportData(int count)
        {
            var atenciones = new List<AtencionReporteItemDTO>();
            for (int i = 1; i <= count; i++)
            {
                atenciones.Add(new AtencionReporteItemDTO
                {
                    Numero = i,
                    Hora = new TimeSpan(7 + (i / 60), i % 60, 0),
                    PacienteNombre = $"Paciente Test {i}",
                    MedicoNombre = $"Medico Test {i % 10}",
                    Status = "Programada",
                    Tipo = "Consulta General",
                    Especialidad = "NA",
                    Asigno = "Sistema"
                });
            }

            return new ReporteDiarioAtencionesMedicasDTO
            {
                Fecha = DateTime.Today,
                ClinicaNombre = "Test Clinic",
                ImpresoPor = "Test User",
                TotalAtenciones = count,
                Atenciones = atenciones
            };
        }
    }
}