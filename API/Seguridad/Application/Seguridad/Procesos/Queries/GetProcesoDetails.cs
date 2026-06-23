using System;
using System.Threading;
using System.Threading.Tasks;
using API.Seguridad.Domain;
using API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using API.Seguridad.DTOs.Seguridad;
using API.Seguridad.Application.Core;

namespace API.Seguridad.Application.Seguridad.Procesos.Queries
{
    public class GetProcesoDetails
    {
        public class Query : IRequest<Result<ProcesoDto>>
        {
            public int Id { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<ProcesoDto>>
        {
            private readonly AppDbContext _context;

            public Handler(AppDbContext context)
            {
                _context = context;
            }

            public async Task<Result<ProcesoDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var proceso = await _context.Procesos
                    .Include(p => p.Subprocesos) // Incluye los procesos hijos
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (proceso == null)
                {
                    return Result<ProcesoDto>.Failure("No se encontró el proceso", 404);
                }

                // Mapear Proceso a ProcesoDto
                var procesoDto = new ProcesoDto
                {
                    Id = proceso.Id,
                    Descr = proceso.Descr,
                    Tipo = proceso.Tipo,
                    Activo = proceso.Activo,
                    Ruta = proceso.Ruta,
                    Icono = proceso.Icono,
                    ProcesoPadreId = proceso.ProcesoPadreId,
                    SistemaId = proceso.SistemaId,
                    Subprocesos = proceso.Subprocesos.Select(sp => new ProcesoDto
                    {
                        Id = sp.Id,
                        Descr = sp.Descr,
                        Tipo = sp.Tipo,
                        ProcesoPadreId = sp.ProcesoPadreId,
                        Activo = sp.Activo
                    }).ToList()
                };

                return Result<ProcesoDto>.Success(procesoDto);
            }
        }
    }
}