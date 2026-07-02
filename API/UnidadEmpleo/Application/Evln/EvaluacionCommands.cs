using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Application.Seguridad.Usuarios.Queries;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Application.Evln
{
    public class EvaluacionCreate
    {
        
        public class Command : IRequest<Result<int>>
        {

            public required string Username { get; set; }
            public required string Password { get; set; }
            public int Id { get; set; }
            public required DateTime Ingreso { get; set; }
           // public DateTime? Salida { get; set; }
            public Boolean? Resultad { get; set; }
            public string Observaciones { get; set; }
            public Boolean? revalorable { get; set; }
            public required int IdSoliciud { get; set; }
            public required TipoEvaluacion TipoEvaluacion { get; set; }
            public string NombreUsuarioEvaluo { get; set; }
            public string UsuarioEvaluo { get; set; }
            public string UsuarioSalida { get; set; }
            public string UsuarioIngreso { get; set; }

        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, AppDbContext context, IMapper _mapper, ILogger<Handler> _logger,
            IHttpContextAccessor http, IMediator mediator) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<int>.Failure("Los datos de la EVALUACIÓN no pueden ser nulos.", 400);

                await using var dbContext = await _factory.CreateAsync();

                // Inicia la transacción
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var entidad = _mapper.Map<Evaluacion>(request);
                    entidad.Ingreso = DateTime.Now;
                    dbContext.Set<Evaluacion>().Add(entidad);
                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;
                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<int>.Failure("Error al crear la EVALUACIÓN", 400);
                    }
                    await transaction.CommitAsync(cancellationToken);
                    return Result<int>.Success(entidad.Id);
                }

                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al crear la EVALUACIÓN "+sqlEx.Number);
                    string mensaje = "";
                    int codigo = 500;
                    switch (sqlEx.Number)
                    {
                        case 2627:
                            Console.WriteLine("Error: El registro ya existe en la base de datos.");
                            mensaje = "Error: Ya existe una Solicitud";
                            break;
                    }

                    return Result<int>.Failure(mensaje, codigo);
                }
                
            }
        }
    }



    public class EvaluacionUpdate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int opcion { get; set; }
            public  int IdRequest { get; set; }
            public required string Username { get; set; }
            public required string Password { get; set; }
            public required int Id { get; set; }
            public required DateTime Ingreso { get; set; }
            public required DateTime? Salida { get; set; }
            public required Boolean Resultado { get; set; }
            public required string Observaciones { get; set; }
            public required Boolean revalorable { get; set; }
            public required int IdSoliciud { get; set; }
            public required TipoEvaluacion TipoEvaluacion { get; set; }
            public string? UsuarioSalida { get; set; } 

            public required string UsuarioEvaluo { get; set; }
            public required string NombreUsuarioEvaluo { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<Unit>.Failure("Los datos de la Evaluación no pueden ser nulos.", 400);

                if (request.Id != request.IdRequest)
                    return Result<Unit>.Failure("El identificador no coincide con el contenido.", 400);

                await using var context = await _factory.CreateAsync();

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var ente = await context.Set<Evaluacion>().FindAsync([request.Id], cancellationToken);

                    if (ente == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("No se encontró la Evaluación", 404);
                    }

                    Boolean cambios = false;
                    if (ente.Salida != request.Salida)
                    {
                        ente.Salida = request.Salida;
                        cambios = true;
                    }
                    if (ente.Resultado != request.Resultado)
                    {
                        ente.Resultado = request.Resultado;
                        cambios = true;
                    }
                    if (!ente.Observaciones.Equals(request.Observaciones))
                    {
                        ente.Observaciones = request.Observaciones;
                        cambios = true;
                    }
                    
                    if (ente.revalorable != request.revalorable)
                    {
                        ente.revalorable = request.revalorable;
                        cambios = true;
                    }
                    
                    if (!cambios)
                        Result<Unit>.Success(Unit.Value);

                    if (!request.UsuarioSalida.Equals(""))
                        ente.UsuarioSalida = request.UsuarioSalida;

                    if (!request.UsuarioEvaluo.Equals(""))
                    {
                        ente.UsuarioEvaluo = request.UsuarioEvaluo;
                        ente.NombreUsuarioEvaluo = request.NombreUsuarioEvaluo;
                    }

                    var result = await context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("Error al actualizar la Evaluación", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return Result<Unit>.Success(Unit.Value);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al actualizar la Evaluación: {Id:}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al actualizar la Evaluación: {request.Id}", 500);
                }
            }
        }
    }

    public class EvaluacionDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using var context = await _factory.CreateAsync();
                try
                {
                    var entidad = await context.Set<Evaluacion>().FindAsync([request.Id], cancellationToken); //.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    if (entidad == null)
                        return Result<Unit>.Failure("No se encontró la SOLICITUD", 404);
                    context.Remove(entidad);
                    var result = await context.SaveChangesAsync(cancellationToken) > 0;
                    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("No fue posible eliminar la SOLICITUD", 400);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error de base de datos al eliminar la SOLICITUD: {request.Id}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al eliminar la SOLICITUD: {request.Id}", 500);
                }
            }
        }
    }


    public class EvaluacionValidaUsuario
    {
        public class Query : IRequest<Result<Usuario>>        {
            public string Id { get; set; }            
        }

        public class Handler(AppDbContext context) : IRequestHandler<Query, Result<Usuario>>
        {
            public async Task<Result<Usuario>> Handle(Query request, CancellationToken cancellationToken)
            {
                
                var user = await context
                    .Usuarios
                    .FirstOrDefaultAsync(f => f.AppUserIdentityId == request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (user == null)
                    return Result<Usuario>.Failure("No existe el usuario "+request.Id, 400);

                return Result<Usuario>.Success(user);
            }
        }

    }

    public class EvaluacionTerminada
    {
        public class Query : IRequest<Result<Usuario>>
        {
            public string Id { get; set; }
        }

        public class Handler(AppDbContext context) : IRequestHandler<Query, Result<Usuario>>
        {
            public async Task<Result<Usuario>> Handle(Query request, CancellationToken cancellationToken)
            {

                var user = await context
                    .Usuarios
                    .FirstOrDefaultAsync(f => f.AppUserIdentityId == request.Id, cancellationToken)
                    .ConfigureAwait(false);

                if (user == null)
                    return Result<Usuario>.Failure("No existe el usuario " + request.Id, 400);

                return Result<Usuario>.Success(user);
            }
        }

    }

}
