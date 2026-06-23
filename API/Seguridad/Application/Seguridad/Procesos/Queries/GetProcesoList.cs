using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Domain;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace API.Seguridad.Application.Seguridad.Procesos.Queries
{
    public class GetSistemaDto
    {
        public SistemaDto getDto(short id, AppDbContext context, CancellationToken cancellationToken)
        {

            var baseQuery = context.Set<Sistema>().AsNoTracking().Where(x => x.Id == id);

            Sistema? p = baseQuery.First();
            if (p == null)
                return null;
            
            SistemaDto item = new SistemaDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Activo = p.Activo
                };

            return item;
        }
    }
    public class GetProcesoList
    {
        public class Query : IRequest<List<ProcesoDto>>
        {
        }

        public class Handler : IRequestHandler<Query, List<ProcesoDto>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }


            public async Task<List<ProcesoDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                List<ProcesoDto>? retorno = await _context.Procesos
                    .Include(p => p.Subprocesos) // Load child processes
                    
                    .Select(p => new ProcesoDto
                    {
                        Id = p.Id,
                        Descr = p.Descr,
                        Tipo = p.Tipo,
                        Activo = p.Activo,
                        Ruta = p.Ruta,
                        Icono = p.Icono,
                        ProcesoPadreId = p.ProcesoPadreId,
                        SistemaId = p.SistemaId,
                        Subprocesos = p.Subprocesos.Select(sp => new ProcesoDto
                        {
                            Id = sp.Id,
                            Descr = sp.Descr,
                            Tipo = sp.Tipo,
                            Activo = sp.Activo
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);

                GetSistemaDto gsd = new GetSistemaDto();

                foreach (ProcesoDto d in retorno)
                {
                    d.Sistema = gsd.getDto(d.SistemaId, _context, cancellationToken);
                }

                return retorno;
            }
        }
    }
}