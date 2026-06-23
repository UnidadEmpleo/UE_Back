using API.Common.Domain;
using API.Common.Util;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Application.AspiranteApp
{
 
    public class AspiranteCreate
    {
        public class Command : IRequest<Result<int>>
        {
            //public string corpoId { get; set; }
            
            public required string Curp { get; set; }
            
            public string Rfc { get; set; }
            
            public required string Nombre { get; set; }
            
            public string Apellido_Paterno { get; set; }
            
            public string Apellido_Materno { get; set; }
            public required DateOnly Fecha_Nacimiento { get; set; }
            public Sexo Sexo { get; set; }
            public string TelefonoCelular { get; set; }
            public int? Foto { get; set; }
            public EdoCivil Estado_Civil { get; set; }
            public GradoEscolaridad Grado_Escolaridad { get; set; }
            public EstadoEscolaridad EscolaridadConcluidaTrunca { get; set; }
            public string DocumentoAcreditaEscolaridad { get; set; }
            public Boolean PensionaodISSEMYM { get; set; }

            public string Calle { get; set; }
            public string EntreCalles { get; set; }
            public string numero { get; set; }
            public string numeroInterior { get; set; }
            public float Latitud { get; set; }
            public float Longitud { get; set; }
            public string Pais { get; set; }
            public int CodigoPostal { get; set; }
            public string Estado { get; set; }
            public string Municipio { get; set; }
            public string Colonia { get; set; }
            public string TipoColonia { get; set; }
            required public Situacion Situacion { get; set; }
            public string IdCuerpoCaptura { get; set; }
            public int IdRegionCaptura { get; set; }
            
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper, ILogger<Handler> _logger,
            IHttpContextAccessor http, IMediator mediator) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<int>.Failure("Los datos del ASPIRANTE no pueden ser nulos.", 400);

                await using var dbContext = await _factory.CreateAsync();

                var conn = dbContext.Database.GetDbConnection().ConnectionString;
                Console.WriteLine(conn);

                //dbContext.Database.CreateExecutionStrategy();

                // Inicia la transacción
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
                try
                {
                    var entidad = _mapper.Map<Aspirante>(request);

                    dbContext.Set<Aspirante>().Add(entidad);

                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<int>.Failure("Error al crear al ASPIRANTE", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);

                    //conn = dbContext.Database.GetDbConnection().ConnectionString;
                    //Console.WriteLine(conn);

                    return Result<int>.Success(1);
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al crear al Aspirante");
                    string mensaje = "";
                    int codigo = 500;
                    switch (sqlEx.Number)
                    {
                        case 2627:
                            Console.WriteLine("Error: El registro ya existe en la base de datos.");
                            mensaje = "Error: Ya existe un aspirante con esa CURP ";
                            break;
                            
                    }

                    return Result<int>.Failure(mensaje, codigo);
                }
            }
        }
    }


    public class AspiranteUpdate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public required string  CurpRequest { get; set; }
            public required string Curp { get; set; }

            public string Rfc { get; set; }

            public required string Nombre { get; set; }

            public string Apellido_Paterno { get; set; }

            public string Apellido_Materno { get; set; }
            public required DateOnly Fecha_Nacimiento { get; set; }
            public Sexo Sexo { get; set; }
            public string TelefonoCelular { get; set; }
            public int? Foto { get; set; }
            public EdoCivil Estado_Civil { get; set; }
            public GradoEscolaridad Grado_Escolaridad { get; set; }
            public EstadoEscolaridad EscolaridadConcluidaTrunca { get; set; }
            public string DocumentoAcreditaEscolaridad { get; set; }
            public Boolean PensionaodISSEMYM { get; set; }

            public string Calle { get; set; }
            public string EntreCalles { get; set; }
            public string numero { get; set; }
            public string numeroInterior { get; set; }
            public float Latitud { get; set; }
            public float Longitud { get; set; }
            public string Pais { get; set; }
            public int CodigoPostal { get; set; }
            public string Estado { get; set; }
            public string Municipio { get; set; }
            public string Colonia { get; set; }
            public string TipoColonia { get; set; }
            public Situacion Situacion { get; set; }
            public string IdCuerpoCaptura { get; set; }
            public int IdRegionCaptura { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<Unit>.Failure("Los datos del ASPIRANTE no pueden ser nulos.", 400);

                if (request.Curp != request.CurpRequest)
                    return Result<Unit>.Failure("El curp no coincide con los datos del contenido.", 400);

                await using var context = await _factory.CreateAsync();

                await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var ente = await context.Set<Aspirante>().FindAsync([request.Curp], cancellationToken);

                    if (ente == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("No se encontró al Aspirante", 404);
                    }

                    if (!Utils.ComparaCambiosAspirante(request, ente))
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Success(Unit.Value);
                    }


                    //ente.FechaUltimaActualizacion = DateTime.UtcNow;
                    _mapper.Map(request, ente);

                    var result = await context.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("Error al actualizar al ASPIRANTE", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return Result<Unit>.Success(Unit.Value);
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al crear al Aspirante");
                    string mensaje = "";
                    int codigo = 500;
                    switch (sqlEx.Number)
                    {
                        case 2627:
                            Console.WriteLine("Error: El registro ya existe en la base de datos.");
                            mensaje = "Error: Ya existe un aspirante con la CURP ";
                            break;
                        case 2601: // Violación de índice UNIQUE (Duplicado)
                            Console.WriteLine("Error: El registro ya existe en la base de datos.");
                            mensaje = "Error: Ya existe un aspirante con esa CURP ";
                            break;

                        case 547: // Violación de clave foránea (Foreign Key) o restricción CHECK
                            mensaje = "Error: Operación inválida debido a una restricción de relación.";
                            break;

                        case 2628: // Los datos de tipo cadena o binarios se truncarían (Texto muy largo)
                            mensaje = "Error: Uno de los campos supera el límite de caracteres permitido.";
                            break;

                        default:
                            mensaje = "Error de SQL Server no controlado. Código: {"+sqlEx.Number+"}";
                            break;

                    }

                    return Result<Unit>.Failure(mensaje, codigo);
                }
            }
        }
    }

    public class AspiranteDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Curp { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                await using var context = await _factory.CreateAsync();
                try
                {
                    var entidad = await context.Set<Aspirante>().FirstOrDefaultAsync(x => x.Curp == request.Curp, cancellationToken);
                    if (entidad == null)
                        return Result<Unit>.Failure("No se encontró al Aspirante", 404);
                    context.Remove(entidad);
                    var result = await context.SaveChangesAsync(cancellationToken) > 0;
                    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("No fue posible eliminar al Aspirante", 400);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error de base de datos al eliminar al Aspirante: {Aspirante}", request.Curp);
                    return Result<Unit>.Failure($"Error de base de datos al eliminar al Aspirante: {request.Curp}", 500);
                }
            }
        }
    }


}