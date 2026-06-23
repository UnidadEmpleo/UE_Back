using API.Application.Reportes.Commands;
using API.DTOs.Reportes;
using API.Seguridad.Application.Core;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace API.Tests.Integration
{
    public class ReporteControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public ReporteControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
            // Add authentication token if needed
        }

        [Fact]
        public async Task ExportarReporteDiario_ShouldReturnPDFFile()
        {
            // Arrange
            var command = new ExportarReporteDiarioAtenciones.Command
            {
                FechaInicio = DateTime.Today,
                Formato = TiposReporte.PDF
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/reporte/atenciones/exportar", command);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Content.Headers.ContentType?.MediaType.Should().Be("application/pdf");
            
            var content = await response.Content.ReadAsByteArrayAsync();
            content.Should().NotBeEmpty();
        }

        [Fact]
        public async Task PreviewReporteDiario_ShouldReturnJSON()
        {
            // Arrange
            var fechaInicio = DateTime.Today.ToString("yyyy-MM-dd");

            // Act
            var response = await _client.GetAsync($"/api/reporte/atenciones/preview?fechaInicio={fechaInicio}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadFromJsonAsync<Result<ReporteDiarioAtencionesMedicasDTO>>();
            content.Should().NotBeNull();
        }
    }
}