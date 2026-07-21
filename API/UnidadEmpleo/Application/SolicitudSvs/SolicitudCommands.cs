using API.Common.Util;
using API.Seguridad.Application.Core;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using iText.Commons.Actions.Contexts;
using MediatR;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace API.UnidadEmpleo.Application.SolicitudSvs
{
    public class SolicitudCreate
    {
        public class Command : IRequest<Result<int>>
        {
            public int Id { get; set; }
            public DateOnly FechaSolicitud { get; set; }
            public Boolean StatusExp { get; set; }
            public Boolean Revalorable { get; set; }
            public StatusSolicitud Status { get; set; }
            public string Observaciones { get; set; }
            public string CorporacionId { get; set; }
            public int RegionId { get; set; }
            public string Curp { get; set; }
            
            
            public string TelefonoCasa { get; set; }
            [MaxLength(10)]
            public string TelefonoRecado { get; set; }
            public EnteraEmpleo enteraEmpleo { get; set; }

            //ULTIMO EMPLEO
            public Boolean Gobierno { get; set; }
            public Boolean Privada { get; set; }
            public string NombreEmpresa { get; set; }
            public string DescripcionEmpresa { get; set; }
            public string Puesto { get; set; }
            public string JefeInmediato { get; set; }
            public string TelefonoEmpleo { get; set; }
            public DateOnly FechaInicio { get; set; }
            public DateOnly FechaFinal { get; set; }
            public string MotivoBaja { get; set; }

            public Boolean Policia { get; set; }
            public string GradoInicioPolicia { get; set; }
            public string GradoFinalPolicia { get; set; }


            public Boolean Militar { get; set; }
            public string GradoInicioMilitar { get; set; }
            public string GradoFinalMilitar { get; set; }

            //EXPEDIENTE
            
            public string coordenadasVivienda { get; set; }
            public Boolean Croquis { get; set; }
            public Boolean DependienteEconomico { get; set; }
            public Boolean CartillaLiberada { get; set; }
            public Boolean CertificadoEstudios { get; set; }
            public Boolean ActaNacimiento { get; set; }
            public Boolean NoAntecedentesPenales { get; set; }
            public Boolean ComprobanteDomicilio { get; set; }
            public Boolean CartasRecomendacion { get; set; }
            public Boolean CurpActualizado { get; set; }
            public Boolean Ine { get; set; }
            public Boolean RfcHomoclave { get; set; }

            //Nuevos se agregaron el 3 06  2026

            public Boolean tarjetaEnvio { get; set; }
            public Boolean presolicitud { get; set; }
            public Boolean fotografias { get; set; }
            public Boolean referenciasDomicilio { get; set; }

            public int notarjetaEnvio { get; set; }
            public int nopre_cartillaLiberada { get; set; }
            public int nocertificadoEstudios { get; set; }
            public int noactaNacimiento { get; set; }
            public int nonoAntecedentesPenales { get; set; }
            public int nocomprobanteDomicilio { get; set; }
            public int nocurpActualizado { get; set; }
            public int noine { get; set; }
            public int norfcHomoclave { get; set; }

            //public List<Referencia> Referencias { get; set; }
            //public List<Evaluacion> Evaluaciones { get; set; }
            //public List<CartaCompromiso> CartasCompromiso { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper _mapper, ILogger<Handler> _logger,
            IHttpContextAccessor http, IMediator mediator) : IRequestHandler<Command, Result<int>>
        {
            public async Task<Result<int>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<int>.Failure("Los datos de la SOLICITUD no pueden ser nulos.", 400);
                int id = 0;
                var strategy = dbContext.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync();
                    try
                    {
                        var entidad = _mapper.Map<Solicitud>(request);

                        dbContext.Set<Solicitud>().Add(entidad);

                        var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return Result<int>.Failure("Error al crear la Solicitud", 400);
                        }

                        await transaction.CommitAsync(cancellationToken);
                        id = entidad.Id;
                        return Result<int>.Success(id);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(ex, "Error de base de datos al crear al Aspirante");
                        return Result<int>.Failure($"Error de base de datos al crear al Aspirante:", 500);
                    }
                });

                return Result<int>.Success(id);
            }
        }
    }


    public class SolicitudUpdate
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int IdRequest { get; set; }
            public int Id { get; set; }
            public DateOnly FechaSolicitud { get; set; }
            public Boolean StatusExp { get; set; }
            public Boolean Revalorable { get; set; }
            public StatusSolicitud Status { get; set; }
            public string Observaciones { get; set; }
            public string CorporacionId { get; set; }
            public int RegionId { get; set; }
            public string Curp { get; set; }

            public string TelefonoCasa { get; set; }
            [MaxLength(10)]
            public string TelefonoRecado { get; set; }
            public EnteraEmpleo enteraEmpleo { get; set; }

            //ULTIMO EMPLEO
            public Boolean Gobierno { get; set; }
            public Boolean Privada { get; set; }
            public string NombreEmpresa { get; set; }
            public string DescripcionEmpresa { get; set; }
            public string Puesto { get; set; }
            public string JefeInmediato { get; set; }
            public string TelefonoEmpleo { get; set; }
            public DateOnly FechaInicio { get; set; }
            public DateOnly FechaFinal { get; set; }
            public string MotivoBaja { get; set; }

            public Boolean Policia { get; set; }
            public string GradoInicioPolicia { get; set; }
            public string GradoFinalPolicia { get; set; }


            public Boolean Militar { get; set; }
            public string GradoInicioMilitar { get; set; }
            public string GradoFinalMilitar { get; set; }

            //EXPEDIENTE

            public string coordenadasVivienda { get; set; }
            public Boolean Croquis { get; set; }
            public Boolean DependienteEconomico { get; set; }
            public Boolean CartillaLiberada { get; set; }
            public Boolean CertificadoEstudios { get; set; }
            public Boolean ActaNacimiento { get; set; }
            public Boolean NoAntecedentesPenales { get; set; }
            public Boolean ComprobanteDomicilio { get; set; }
            public Boolean CartasRecomendacion { get; set; }
            public Boolean CurpActualizado { get; set; }
            public Boolean Ine { get; set; }
            public Boolean RfcHomoclave { get; set; }

            //Nuevos se agregaron el 3 06  2026

            public Boolean tarjetaEnvio { get; set; }
            public Boolean presolicitud { get; set; }
            public Boolean fotografias { get; set; }
            public Boolean referenciasDomicilio { get; set; }

            public int notarjetaEnvio { get; set; }
            public int nopre_cartillaLiberada { get; set; }
            public int nocertificadoEstudios { get; set; }
            public int noactaNacimiento { get; set; }
            public int nonoAntecedentesPenales { get; set; }
            public int nocomprobanteDomicilio { get; set; }
            public int nocurpActualizado { get; set; }
            public int noine { get; set; }
            public int norfcHomoclave { get; set; }

        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                if (request == null)
                    return Result<Unit>.Failure("Los datos de la SOLICITUD no pueden ser nulos.", 400);

                if (request.Id != request.IdRequest)
                    return Result<Unit>.Failure("El identificador de petición no coincide con el del registgro.", 400);

                var strategy = dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    await using var transaction = await dbContext.Database.BeginTransactionAsync();

                 
                    // Operaciones
                    try
                    {
                        var ente = await dbContext.Set<Solicitud>().FindAsync([request.Id], cancellationToken);

                        if (ente == null)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return Result<Unit>.Failure("No se encontró la SOLICITUD", 404);
                        }

                        ente.FechaUltimaActualizacion = DateTime.UtcNow;
                        Boolean cambios = Utils.Compara(request, ente);

                        if (!cambios)
                            return Result<Unit>.Failure("Sin cambios, puede continuar", 400);

                        // old = 5, new = 2 

                        StatusSolicitud tmpOldStatus = ente.Status;
                        int tmpNewStatus = (int)request.Status;

                        _mapper.Map(request, ente);

                        // Condición derivado de la actualización por 
                        // Si el estado de la petición sea menor a dos dejar el que esta, de lo contrario poner la actualización
                        if (tmpNewStatus <= 2 && (int)tmpOldStatus >= 3) //& (int)request.Status > (int)ente.Status)  )
                            ente.Status = tmpOldStatus;

                        var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                        if (!result)
                        {
                            await transaction.RollbackAsync(cancellationToken);
                            return Result<Unit>.Failure("Error al actualizar la SOLICITUD", 400);
                        }

                        await transaction.CommitAsync(cancellationToken);
                       
                    }
                    catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        _logger.LogError(ex, "Error de base de datos al crear la SOLICITD");
                        string mensaje = "";
                        int codigo = 500;
                        switch (sqlEx.Number)
                        {
                            case 2627:
                                Console.WriteLine("Error: El registro ya existe en la base de datos.");
                                mensaje = "Error: Ya existe una Solicitud";
                                break;
                        }

                        return Result<Unit>.Failure(mensaje, codigo);
                    }

                    return Result<Unit>.Success(Unit.Value);

                });

                return Result<Unit>.Success(Unit.Value);
               
            }
        }
    }

    public class SolicitudDelete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {                
                try
                {
                    var entidad = await dbContext.Set<Solicitud>().FindAsync([request.Id], cancellationToken); //.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                    if (entidad == null)
                        return Result<Unit>.Failure("No se encontró al Aspirante", 404);
                    dbContext.Remove(entidad);
                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;
                    return result ? Result<Unit>.Success(Unit.Value) : Result<Unit>.Failure("No fue posible eliminar al Aspirante", 400);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error de base de datos al eliminar la SOLICITUD: {request.Id}", request.Id);
                    return Result<Unit>.Failure($"Error de base de datos al eliminar la SOLICITUD: {request.Id}", 500);
                }
            }
        }
    }

    public class SolicitudUpdateStatus
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int IdSolicitud { get; set; }            
            public Boolean Revalorable { get; set; }
            public StatusSolicitud Status { get; set; }
            
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper _mapper, ILogger<Handler> _logger) : IRequestHandler<Command, Result<Unit>>
        {
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {                
                //await using var context = await _factory.CreateAsync();
                await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

                try
                {
                    var ente = await dbContext.Set<Solicitud>().FindAsync([request.IdSolicitud], cancellationToken);

                    if (ente == null)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("No se encontró la SOLICITUD", 404);
                    }

                    ente.FechaUltimaActualizacion = DateTime.UtcNow;
                    ente.Status = request.Status;
                    ente.Revalorable = request.Revalorable;

                    var result = await dbContext.SaveChangesAsync(cancellationToken) > 0;

                    if (!result)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return Result<Unit>.Failure("Error al actualizar la SOLICITUD", 400);
                    }

                    await transaction.CommitAsync(cancellationToken);
                    return Result<Unit>.Success(Unit.Value);
                }
                catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    _logger.LogError(ex, "Error de base de datos al crear la SOLICITD");
                    string mensaje = "";
                    int codigo = 500;
                    switch (sqlEx.Number)
                    {
                        case 2627:
                            Console.WriteLine("Error: El registro ya existe en la base de datos.");
                            mensaje = "Error: Ya existe una Solicitud";
                            break;
                    }

                    return Result<Unit>.Failure(mensaje, codigo);
                }
            }
        }
    }
}
