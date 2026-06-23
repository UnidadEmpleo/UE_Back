using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Application.Core;

namespace API.Seguridad.Application.Seguridad.Procesos.Commands
{
    public class CreateProceso
    {
        public class Command : IRequest<Result<int>>
        {
            public string Descr { get; set; }
            public string Tipo { get; set; }
            public string? Icono { get; set; }
            public int? ProcesoPadreId { get; set; } // ID del proceso padre (si es un subproceso)  
            public List<int> SubProcesoIds { get; set; } = []; // IDs de subprocesos existentes
            public short SistemaId { get; set; }
            public string? Ruta { get; set; }
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Tipo.Equals("P", StringComparison.OrdinalIgnoreCase) &&
                    request.SubProcesoIds?.Count > 0)
                {
                    return Result<int>.Failure($"El proceso {request.Descr} no debe contener subprocesos", 400);
                }

                // Validate sub-process IDs
                if (request.SubProcesoIds != null && request.SubProcesoIds.Any())
                {
                    var existingSubProcesos = await context.Procesos
                        .Where(p => request.SubProcesoIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    var invalidIds = request
                        .SubProcesoIds
                        .Except(existingSubProcesos.Select(s => s.Id))
                        .ToList();

                    if (invalidIds.Any())
                    {
                        return Result<int>.Failure($"Los siguientes IDs de subprocesos no son válidos: {string.Join(", ", invalidIds)}", 400);
                    }

                    if (existingSubProcesos.Any(s => s.Tipo.Equals("A", StringComparison.OrdinalIgnoreCase)))
                    {
                        return Result<int>.Failure($"Los subprocesos no pueden ser de tipo Agrupadores", 400);
                    }
                }

                // Create the main process using AutoMapper
                var proceso = mapper.Map<Proceso>(request);
                proceso.ProcesoPadreId = request.ProcesoPadreId.HasValue && 
                                         request.Tipo.Equals("P", StringComparison.OrdinalIgnoreCase) ? 
                    request.ProcesoPadreId : 
                    null;

                context.Procesos.Add(proceso);

                // Associate valid sub-processes
                if (request.SubProcesoIds != null && request.SubProcesoIds.Any())
                {
                    var subProcesos = await context.Procesos
                        .Where(p => request.SubProcesoIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    proceso.Subprocesos = subProcesos;
                }

                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<int>.Failure("Error al crear el proceso", 400);
                }

                return Result<int>.Success(proceso.Id);
            }
        }
    }

    public class DeleteProceso
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(AppDbContext context) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var proceso = await context
                    .Procesos
                    .FindAsync([request.Id], cancellationToken);

                if (proceso == null)
                {
                    return Result<Unit>.Failure("No se encontró el proceso", 404);
                }

                proceso.Activo = false;

                int rowsAfected = await context.SaveChangesAsync(cancellationToken);

                var result = rowsAfected > 0;

                if (!result)
                {
                    return Result<Unit>.Failure("Error al des activar el proceso", 400);
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }

   public class UpdateProceso
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
            public string Descr { get; set; }
            public string Tipo { get; set; }
            public string Icono { get; set; }
            public bool Activo { get; set; } = true;
            public string? Ruta { get; set; }
            public int? ProcesoPadreId { get; set; } // ID del proceso padre (si es un subproceso)
            public short SistemaId { get; set; }
            public List<int> SubProcesoIds { get; set; } = []; // IDs de subprocesos existentes
        }

        public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request.Tipo.Equals("P", StringComparison.OrdinalIgnoreCase) &&
                    request.SubProcesoIds?.Count > 0)
                {
                    return Result<Unit>.Failure($"El proceso {request.Descr} no debe contener subprocesos", 400);
                }

                

                var proceso = await context
                    .Procesos
                    .Include(p => p.Subprocesos) // Incluir la relación de subprocesos
                    .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

                if (proceso == null)
                {
                    return Result<Unit>.Failure("No se encontró el proceso", 404);
                }

                // Map request to existing entity using AutoMapper
                mapper.Map(request, proceso);
                proceso.FechaUltimaActualizacion = DateTime.UtcNow;
                proceso.ProcesoPadreId = request.ProcesoPadreId.HasValue &&
                                         request.Tipo.Equals("P", StringComparison.OrdinalIgnoreCase) ?
                                            request.ProcesoPadreId :
                                            null;

                // Validar y actualizar los subprocesos
                if (request.SubProcesoIds != null && request.SubProcesoIds.Any())
                {
                    var existingSubProcesos = await context.Procesos
                        .Where(p => request.SubProcesoIds.Contains(p.Id))
                        .ToListAsync(cancellationToken);

                    var invalidIds = request
                        .SubProcesoIds
                        .Except(existingSubProcesos.Select(s => s.Id))
                        .ToList();

                    if (invalidIds.Any())
                    {
                        return Result<Unit>.Failure($"Los siguientes IDs de subprocesos no son válidos: {string.Join(", ", invalidIds)}", 400);
                    }

                    if (existingSubProcesos.Any(s => s.Tipo.Equals("A", StringComparison.OrdinalIgnoreCase)))
                    {
                        return Result<Unit>.Failure($"Los subprocesos no pueden ser de tipo Agrupadores", 400);
                    }

                    // Actualizar la colección de subprocesos
                    proceso.Subprocesos = existingSubProcesos;
                }
                else
                {
                    // Si no se proporcionan IDs, limpiar la colección de subprocesos
                    proceso.Subprocesos.Clear();
                }

                //context.Update(proceso);
                var result = await context.SaveChangesAsync(cancellationToken) > 0;

                if (!result)
                {
                    return Result<Unit>.Failure("Error al actualizar el proceso", 400);
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }        
}