using API.Application.Reportes.Services.PDF;
using API.DTOs.Reportes;
using FluentAssertions;
using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace API.Tests.Performance
{
    public class ReportGenerationPerformanceTests
    {
        private readonly ITestOutputHelper _output;

        public ReportGenerationPerformanceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task PDFGeneration_ShouldCompleteInReasonableTime(int recordCount)
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();
            var data = CreateReportData(recordCount);
            var stopwatch = Stopwatch.StartNew();

            // Act
            var result = await generator.GenerateAsync(data);
            stopwatch.Stop();

            // Assert
            result.Should().NotBeEmpty();
            _output.WriteLine($"Generated PDF with {recordCount} records in {stopwatch.ElapsedMilliseconds}ms");
            
            // Performance threshold: should complete in under 5 seconds for 1000 records
            stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000);
        }

        [Fact]
        public async Task PDFGeneration_ShouldHandleConcurrentRequests()
        {
            // Arrange
            var generator = new ReporteDiarioPDFGenerator();
            var data = CreateReportData(100);
            var tasks = new List<Task<byte[]>>();

            // Act
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(generator.GenerateAsync(data));
            }

            var results = await Task.WhenAll(tasks);

            // Assert
            results.Should().HaveCount(10);
            results.Should().OnlyContain(r => r.Length > 0);
        }

        private static ReporteDiarioAtencionesMedicasDTO CreateReportData(int count)
        {
            var atenciones = new List<AtencionReporteItemDTO>();
            for (int i = 1; i <= count; i++)
            {
                atenciones.Add(new AtencionReporteItemDTO
                {
                    Numero = i,
                    Hora = new TimeSpan(7 + (i / 60), i % 60, 0),
                    PacienteNombre = $"Paciente {i}",
                    MedicoNombre = $"Medico {i % 10}",
                    Status = "Programada",
                    Tipo = "Consulta",
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