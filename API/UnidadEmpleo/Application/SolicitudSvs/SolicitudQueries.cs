using API.Common.Domain;
using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.DTOs.Seguridad;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using System.ComponentModel.DataAnnotations;

namespace API.UnidadEmpleo.Application.SolicitudSvs
{


    public class GetSolicitudByPerfil2
    {
        public class Query : IRequest<Result<List<SolicitudDto>>>
        {
            public string cuerpoId { get; set; }
            public int regionId { get; set; }
            public int perfilId { get; set; }
            public int statusSolicitud { get; set; }
            public DateOnly fechainicio { get; set; }
            public DateOnly fechatermino { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext) : IRequestHandler<Query, Result<List<SolicitudDto>>>
        {
            public async Task<Result<List<SolicitudDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                if (request.fechainicio > request.fechatermino)
                {
                    return Result<List<SolicitudDto>>.Failure("La fecha de inicio debe ser menor a la de termino", 100);
                }

                List<SolicitudDto> entidades = new List<SolicitudDto>();

                DateTime utcNow = DateTime.UtcNow;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.Local);
                DateOnly today = DateOnly.FromDateTime(localTime);

                //await using var dbContext = await _factory.CreateAsync();


                // si perfil es administrador o subdirector todo con opciones                  
                //    gerente o atencionregistro, todo de ellos para abajo
                //    capturista solo ellos

                IQueryable<Solicitud> query = dbContext.Solicitud.Include(b => b.Aspirante).AsQueryable();

                if (request.perfilId == (int)TipoRoles.Administrador || request.perfilId == (int)TipoRoles.Subdirector) { 
                    query = query.Where(p =>
                        p.FechaSolicitud >= request.fechainicio &&
                        p.FechaSolicitud <= request.fechatermino);

                    if (request.statusSolicitud >= 0)
                        query = query.Where(p =>p.Status == (StatusSolicitud)request.statusSolicitud);
                    
                    if (request.regionId >= 0)
                        query = query.Where(p =>p.RegionId == request.regionId);
                    
                    if (request.cuerpoId != "TODOS")
                        query = query.Where(p =>p.CorporacionId == request.cuerpoId);
                    /*
                    query.Where(p =>
                            (p.FechaSolicitud >= request.fechainicio)
                            && (p.FechaSolicitud <= request.fechatermino)
                            && (request.statusSolicitud >= 0 || p.Status == (StatusSolicitud)request.statusSolicitud )
                            && (request.regionId >= 0 || p.RegionId == request.regionId)
                            && (request.cuerpoId != "TODOS" || p.CorporacionId == request.cuerpoId)
                            );
                    */
                }
                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO
                else if (request.perfilId == (int)TipoRoles.Gerente || request.perfilId == (int)TipoRoles.AtencionRegistro)
                {
                    query = query.Where(p =>
                        p.FechaSolicitud >= request.fechainicio &&
                        p.FechaSolicitud <= request.fechatermino
                        && p.CorporacionId == request.cuerpoId);

                    if (request.statusSolicitud >= 0)
                        query = query.Where(p => p.Status == (StatusSolicitud)request.statusSolicitud);

                    if (request.regionId >= 0)
                        query = query.Where(p => p.RegionId == request.regionId);

                }
                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CORPORACIÓN Y ESTATUS
                else if (request.perfilId == (int)TipoRoles.Medico || request.perfilId == (int)TipoRoles.Psicologo || request.perfilId == (int)TipoRoles.Antidoping)
                {
                    query = query.Where(p =>
                        p.FechaSolicitud >= request.fechainicio &&
                        p.FechaSolicitud <= request.fechatermino
                        && p.CorporacionId == request.cuerpoId);

                    if (request.statusSolicitud >= 0)
                        query = query.Where(p => p.Status == (StatusSolicitud)request.statusSolicitud);

                    if (request.regionId >= 0)
                        query = query.Where(p => p.RegionId == request.regionId);

                }

                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO Y REGION Y ESTATUS
                else if (request.perfilId == (int)TipoRoles.Capturista)
                {

                    query = query.Where(p =>
                        p.FechaSolicitud >= request.fechainicio 
                        && p.FechaSolicitud <= request.fechatermino
                        && p.CorporacionId == request.cuerpoId
                        && p.RegionId == request.regionId);

                    if (request.statusSolicitud >= 0)
                        query = query.Where(p => p.Status == (StatusSolicitud)request.statusSolicitud);

                }

                entidades = await query.Select(p => new SolicitudDto
                {
                    Id = p.Id,
                    FechaSolicitud = p.FechaSolicitud,
                    StatusExp = p.StatusExp,
                    Revalorable = p.Revalorable,
                    Status = p.Status,
                    Observaciones = p.Observaciones,
                    CorporacionId = p.CorporacionId,
                    RegionId = p.RegionId,
                    Curp = p.Curp,
                    Corporacion = null,
                    Region = null,
                    Aspirante = new AspiranteDtoBasic
                    {
                        Curp = p.Aspirante.Curp,
                        Rfc = p.Aspirante.Rfc,
                        Nombre = p.Aspirante.Nombre,
                        Apellido_Paterno = p.Aspirante.Apellido_Paterno,
                        Apellido_Materno = p.Aspirante.Apellido_Materno,
                        Fecha_Nacimiento = p.Aspirante.Fecha_Nacimiento,
                        Sexo = p.Aspirante.Sexo
                    },
                    TelefonoCasa = p.TelefonoCasa,
                    TelefonoRecado = p.TelefonoRecado,
                    enteraEmpleo = p.enteraEmpleo,
                    //ULTIMO EMPLEO
                    Gobierno = p.Gobierno,
                    Privada = p.Privada,
                    NombreEmpresa = p.NombreEmpresa,
                    DescripcionEmpresa = p.DescripcionEmpresa,
                    Puesto = p.Puesto,
                    JefeInmediato = p.JefeInmediato,
                    TelefonoEmpleo = p.TelefonoEmpleo,
                    FechaInicio = p.FechaInicio,
                    FechaFinal = p.FechaFinal,
                    MotivoBaja = p.MotivoBaja,
                    Policia = p.Policia,
                    GradoInicioPolicia = p.GradoInicioPolicia,
                    GradoFinalPolicia = p.GradoFinalPolicia,
                    Militar = p.Militar,
                    GradoInicioMilitar = p.GradoInicioMilitar,
                    GradoFinalMilitar = p.GradoFinalMilitar,

                    //EXPEDIENTE
                    Fotos = p.Fotos,
                    coordenadasVivienda = p.coordenadasVivienda,
                    Croquis = p.Croquis,
                    DependienteEconomico = p.DependienteEconomico,
                    CartillaLiberada = p.CartillaLiberada,
                    CertificadoEstudios = p.CertificadoEstudios,
                    ActaNacimiento = p.ActaNacimiento,
                    NoAntecedentesPenales = p.NoAntecedentesPenales,
                    ComprobanteDomicilio = p.ComprobanteDomicilio,
                    CartasRecomendacion = p.CartasRecomendacion,
                    CurpActualizado = p.CurpActualizado,
                    Ine = p.Ine,
                    RfcHomoclave = p.RfcHomoclave,

                    tarjetaEnvio = p.tarjetaEnvio,
                    presolicitud = p.presolicitud,
                    fotografias = p.fotografias,
                    referenciasDomicilio = p.referenciasDomicilio,

                    notarjetaEnvio = p.notarjetaEnvio,
                    nopre_cartillaLiberada = p.nopre_cartillaLiberada,
                    nocertificadoEstudios = p.nocertificadoEstudios,
                    noactaNacimiento = p.noactaNacimiento,
                    nonoAntecedentesPenales = p.nonoAntecedentesPenales,
                    nocomprobanteDomicilio = p.nocomprobanteDomicilio,
                    nocurpActualizado = p.nocurpActualizado,
                    noine = p.noine,
                    norfcHomoclave = p.norfcHomoclave
                }).
                ToListAsync(); 
                
                return Result<List<SolicitudDto>>.Success(entidades);
            }

        }



    }


    public class GetSolicitudByPerfil
    {
        public class Query : IRequest<Result<List<Solicitud>>>
        {
            public string cuerpoId { get; set; }
            public int regionId { get; set; }
            public int perfilId { get; set; }
            public int statusSolicitud { get; set; }
            public DateOnly? fechainicio { get; set; }
            public DateOnly? fechatermino { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext) : IRequestHandler<Query, Result<List<Solicitud>>>
        {
            public async Task<Result<List<Solicitud>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();
                var query = dbContext.Solicitud.Include(b => b.Aspirante).AsQueryable();
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                // si perfil es administrador o subdirector todo con opciones                  
                //    gerente o atencionregistro, todo de ellos para abajo
                //    capturista solo ellos

                if (request.perfilId == (int)TipoRoles.Administrador || request.perfilId == (int)TipoRoles.Subdirector)
                {
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    bool status = false;
                    bool reg = false;


                    if (request.statusSolicitud >= 0)
                    {
                        query = query.Where(p => (int)p.Status == request.statusSolicitud);
                        status = true;
                    }

                    if (request.regionId >= 0)
                    {
                        if (!status) { query = query.Where(p => p.RegionId == request.regionId); reg = true; }
                        else
                            query = query.Where(p => p.RegionId == request.regionId && (int)p.Status == request.statusSolicitud);
                    }

                    if (request.cuerpoId != "TODOS")
                    {
                        if (!status && !reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId);
                        else if (status && reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && (int)p.Status == request.statusSolicitud);
                        else if (!status && reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId);
                        else if (status && !reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && (int)p.Status == request.statusSolicitud);
                    }

                    if (request.fechainicio < today)
                    {
                        if (!status && !reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (status && reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && (int)p.Status == request.statusSolicitud && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (!status && reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (status && !reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && (int)p.Status == request.statusSolicitud);
                    }
                }


                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO
                else if (request.perfilId == (int)TipoRoles.Gerente || request.perfilId == (int)TipoRoles.AtencionRegistro)
                {
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    bool status = false;
                    bool reg = false;

                    query = query.Where(p => p.CorporacionId == request.cuerpoId);

                    if (request.statusSolicitud >= 0)
                    {
                        query = query.Where(p => (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId);
                        status = true;
                    }

                    if (request.regionId >= 0)
                    {
                        if (!status) { query = query.Where(p => p.RegionId == request.regionId && p.CorporacionId == request.cuerpoId); reg = true; }
                        else
                            query = query.Where(p => p.RegionId == request.regionId && (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId);
                    }

                    if (request.fechainicio < today)
                    {
                        if (!status && !reg)
                            query = query.Where(p => (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (status && reg)
                            query = query.Where(p => p.RegionId == request.regionId && (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (!status && reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else if (status && !reg)
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && (int)p.Status == request.statusSolicitud && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                    }

                }

                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO Y REGION
                else if (request.perfilId == (int)TipoRoles.Capturista)
                {
                    Boolean status = false;
                    if (request.statusSolicitud >= 0)
                    {
                        query = query.Where(p => (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId);
                        status = true;
                    }
                    else
                        query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId);

                    if (request.fechainicio < today)
                    {
                        if (status)
                            query = query.Where(p => (int)p.Status == request.statusSolicitud && p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                        else
                            query = query.Where(p => p.CorporacionId == request.cuerpoId && p.RegionId == request.regionId && p.FechaInicio >= request.fechainicio && p.FechaFinal <= request.fechatermino);
                    }
                    else
                        query = query.Where(p => p.Curp == "X");
                    
                }
                
                List<Solicitud> entidades = query.ToList();
                return Result<List<Solicitud>>.Success(entidades);
            }
        }
    }

    public class GetSolicitud
    {
        public class Query : IRequest<Result<Solicitud>>
        {
            public int Id { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext, IMapper mapper) : IRequestHandler<Query, Result<Solicitud>>
        {
            public async Task<Result<Solicitud>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();
                var baseQuery = dbContext.Set<Solicitud>().AsNoTracking()
                            .Where(x => x.Id == request.Id)
                            .Include(b => b.Referencias)
                            .Include(b => b.Aspirante);
                Solicitud? queryData = await baseQuery
                    .FirstOrDefaultAsync(cancellationToken);
                if (queryData == null)
                    return Result<Solicitud>.Failure("No se encontró la Solicitud", 404);


                return Result<Solicitud>.Success(queryData);
            }
        }
    }

    public class GetSolicitudList
    {
        public class Query : IRequest<Result<List<Solicitud>>> { }

        public class Handler(UnidadEmpleoDbContext dbContext) : IRequestHandler<Query, Result<List<Solicitud>>>
        {
            public async Task<Result<List<Solicitud>>> Handle(Query request, CancellationToken cancellationToken)
            {
                //await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<Solicitud>().Include(b => b.Aspirante)
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<Solicitud>>.Success(entidades);
            }
        }
    }

    public class GetSolicitudListByAspirante
    {
        public class Query : IRequest<Result<List<SolicitudDto>>> { 
            public string Curp { get; set; }
        }

        public class Handler(UnidadEmpleoDbContext dbContext) : IRequestHandler<Query, Result<List<SolicitudDto>>>
        {
            public async Task<Result<List<SolicitudDto>>> Handle(Query request, CancellationToken cancellationToken)
            {

                List<SolicitudDto> entidades = await dbContext.Set<Solicitud>()
                    .Where(x => x.Curp == request.Curp)
                    .Include(b => b.Aspirante)
                    .Select(p => new SolicitudDto
                    {
                        Id = p.Id,
                        FechaSolicitud = p.FechaSolicitud,
                        StatusExp = p.StatusExp,
                        Revalorable = p.Revalorable,
                        Status = p.Status,
                        Observaciones = p.Observaciones,
                        CorporacionId = p.CorporacionId,
                        RegionId = p.RegionId,
                        Curp = p.Curp,
                        Corporacion = null,
                        Region = null,
                        Aspirante = new AspiranteDtoBasic
                        {
                            Curp = p.Aspirante.Curp,
                            Rfc = p.Aspirante.Rfc,
                            Nombre = p.Aspirante.Nombre,
                            Apellido_Paterno = p.Aspirante.Apellido_Paterno,
                            Apellido_Materno = p.Aspirante.Apellido_Materno,
                            Fecha_Nacimiento = p.Aspirante.Fecha_Nacimiento,
                            Sexo = p.Aspirante.Sexo
                        },
                        TelefonoCasa = p.TelefonoCasa,
                        TelefonoRecado = p.TelefonoRecado,
                        enteraEmpleo = p.enteraEmpleo,
                        //ULTIMO EMPLEO
                        Gobierno = p.Gobierno,
                        Privada = p.Privada,
                        NombreEmpresa = p.NombreEmpresa,
                        DescripcionEmpresa = p.DescripcionEmpresa,
                        Puesto = p.Puesto,
                        JefeInmediato = p.JefeInmediato,
                        TelefonoEmpleo = p.TelefonoEmpleo,
                        FechaInicio = p.FechaInicio,
                        FechaFinal = p.FechaFinal,
                        MotivoBaja = p.MotivoBaja,
                        Policia = p.Policia,
                        GradoInicioPolicia = p.GradoInicioPolicia,
                        GradoFinalPolicia = p.GradoFinalPolicia,
                        Militar = p.Militar,
                        GradoInicioMilitar = p.GradoInicioMilitar,
                        GradoFinalMilitar = p.GradoFinalMilitar,

                        //EXPEDIENTE
                        Fotos = p.Fotos,
                        coordenadasVivienda = p.coordenadasVivienda,
                        Croquis = p.Croquis,
                        DependienteEconomico = p.DependienteEconomico,
                        CartillaLiberada = p.CartillaLiberada,
                        CertificadoEstudios = p.CertificadoEstudios,
                        ActaNacimiento = p.ActaNacimiento,
                        NoAntecedentesPenales = p.NoAntecedentesPenales,
                        ComprobanteDomicilio = p.ComprobanteDomicilio,
                        CartasRecomendacion = p.CartasRecomendacion,
                        CurpActualizado = p.CurpActualizado,
                        Ine = p.Ine,
                        RfcHomoclave = p.RfcHomoclave,

                        tarjetaEnvio = p.tarjetaEnvio,
                        presolicitud = p.presolicitud,
                        fotografias = p.fotografias,
                        referenciasDomicilio = p.referenciasDomicilio,

                        notarjetaEnvio = p.notarjetaEnvio,
                        nopre_cartillaLiberada = p.nopre_cartillaLiberada,
                        nocertificadoEstudios = p.nocertificadoEstudios,
                        noactaNacimiento = p.noactaNacimiento,
                        nonoAntecedentesPenales = p.nonoAntecedentesPenales,
                        nocomprobanteDomicilio = p.nocomprobanteDomicilio,
                        nocurpActualizado = p.nocurpActualizado,
                        noine = p.noine,
                        norfcHomoclave = p.norfcHomoclave

                    })
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<SolicitudDto>>.Success(entidades);
            }
        }
    }

    

    /*
    public class GetPagedAspirantes
    {
        public class Query : IRequest<Result<PagedResult<AspiranteDto>>>
        {
            public string? Nombre { get; set; }
            public string? Curp { get; internal set; }
            public PaginationParams Pagination { get; set; } = new();
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper _mapper) : IRequestHandler<Query, Result<PagedResult<AspiranteDto>>>
        {
            public async Task<Result<PagedResult<AspiranteDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var pagination = request.Pagination ?? new PaginationParams();

                if (pagination.PageNumber <= 0 || pagination.PageSize <= 0)
                {
                    return Result<PagedResult<AspiranteDto>>.Failure("PageNumber and PageSize must be greater than zero", 400);
                }

                var validSortFields = new[] { "Nombre", "Curp", "Apellido Paterno" };
                if (!string.IsNullOrEmpty(pagination.SortBy) && !validSortFields.Contains(pagination.SortBy))
                {
                    return Result<PagedResult<AspiranteDto>>.Failure($"Invalid SortBy field: {pagination.SortBy}", 400);
                }

                await using var context = await _factory.CreateAsync();

                var query = context
                    .Set<API.UnidadEmpleo.Domain.Aspirante>()
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Nombre))
                {
                    var nombreLower = request.Nombre.ToLowerInvariant();
                    query = query.Where(p =>
                        (p.Nombre + " " + p.Apellido_Paterno + " " + (p.Apellido_Materno ?? "")).ToLower().Contains(nombreLower)
                    );
                }

                if (!string.IsNullOrEmpty(request.Curp))
                {
                    var curpLower = request.Curp.ToLowerInvariant();
                    query = query.Where(p => p.Curp.ToLower().Contains(curpLower));
                }

                // Ejecutar COUNT antes de la paginación
                var totalRecords = await query.CountAsync(cancellationToken);

                if (!string.IsNullOrEmpty(pagination.SortBy))
                {
                    query = pagination.SortDirection == "desc"
                        ? query.OrderByDescending(e => EF.Property<object>(e, pagination.SortBy))
                        : query.OrderBy(e => EF.Property<object>(e, pagination.SortBy));
                }
                else
                {
                    //query = query.OrderByDescending(p => p.FechaCreacion);
                }

                var pagedPacientes = await query
                    .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                    .Take(pagination.PageSize)
                    .ToListAsync(cancellationToken);

                var pacienteDtos = _mapper.Map<List<AspiranteDto>>(pagedPacientes);

                var result = new PagedResult<AspiranteDto>
                {
                    Data = pacienteDtos,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)pagination.PageSize),
                    PageNumber = pagination.PageNumber,
                    PageSize = pagination.PageSize
                };

                return Result<PagedResult<AspiranteDto>>.Success(result);
            }
        }
    }*/
}
