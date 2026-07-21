using API.Seguridad.Application.Anexos.Commands;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using iText.Commons.Actions.Contexts;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;

namespace API.UnidadEmpleo.Application.Informes
{
    public class InformeQueries
    {

    }

    public class GetIndicadores
    {

        public class Query : IRequest<Result<List<IndicadorDto>>>
        {
            public string CorporacionId { get; set; }
            public int? regionId { get; set; }
            public DateOnly fechainicio { get; set; }
            public DateOnly fechatermino { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, ILogger<CreateAnexoHandler> _logger) : IRequestHandler<Query, Result<List<IndicadorDto>>>
        {
            public async Task<Result<List<IndicadorDto>>> Handle(Query filtro, CancellationToken cancellationToken)
            {
                
                DateTime utcNow = DateTime.UtcNow;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.Local);
                DateOnly today = DateOnly.FromDateTime(localTime);

                //await using var dbContext = await _factory.CreateAsync();
                var query = dbContext.Solicitud.AsQueryable();

                bool reg = false;
                
                if (filtro.regionId >= 0)
                    reg = true;

                var resultado = await dbContext.Solicitud
                    .Where(x =>
                        (x.FechaSolicitud >= filtro.fechainicio) 
                        && (x.FechaSolicitud <= filtro.fechatermino) 
                        && (filtro.CorporacionId == "TODOS" || x.CorporacionId == filtro.CorporacionId) 
                        && (filtro.regionId == -1 || x.RegionId == filtro.regionId)                        
                        )
                    .GroupBy(x => x.Status)
                    .Select(g => new IndicadorDto
                    {
                        key = (int)g.Key,
                        label = g.Key.ToString(),
                        value = g.Count(),
                        suffix = ""
                    }
                    ).ToListAsync();

                return Result<List<IndicadorDto>>.Success(resultado);
            }
        }
    }

}
