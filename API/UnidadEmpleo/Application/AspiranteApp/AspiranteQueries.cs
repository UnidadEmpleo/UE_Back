using API.Common.Domain;
using API.Seguridad.Application.Core;
using API.Seguridad.Domain.Seguridad;
using API.Seguridad.Infrastructure;
using API.UnidadEmpleo.Domain;
using API.UnidadEmpleo.DTO;
using API.UnidadEmpleo.Persistence;
using API.UnidadEmpleo.Services;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph.Models;
using static QRCoder.PayloadGenerator.SwissQrCode;
using static QuestPDF.Helpers.Colors;

namespace API.UnidadEmpleo.Application.AspiranteApp
{
    /**
     * Capturistas: Lista de Aspirantes por area de captura y estado de captura,
     * Gerencia: Lista de Aspirantes en revisión o captura por la gerencia de todas sus áreas de captura subordinadas (regiones) .
     * Psicológico: Lista de Aspirantes que ya fueron aprovados por la gerencia para pasar a su examen.
     * Médico: Lista de Aspirantes que ya fueron aprovados por la gerencia para pasar a su examen.
     * Antidoping: Lista de Aspirantes que ya fueron aprovados por la gerencia para pasar a su examen.
     */


    public class GetAspiranteCapturaGerencia
    {
        public class Query : IRequest<Result<List<API.UnidadEmpleo.Domain.Aspirante>>> { 
            public string cuerpoId { get; set; }
            public int regionId { get; set; }
            public int perfilId { get; set; }
            public int situacion {  get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<API.UnidadEmpleo.Domain.Aspirante>>>
        {
            public async Task<Result<List<API.UnidadEmpleo.Domain.Aspirante>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                
                var query  = dbContext.Aspirante.AsQueryable();
                // si perfil es administrador o subdirector todo con opciones                  
                //    gerente o atencionregistro, todo de ellos para abajo
                //    capturista solo ellos

                if (request.perfilId == (int)TipoRoles.Administrador || request.perfilId == (int)TipoRoles.Subdirector)
                {
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    bool situ = false;
                    bool reg = false;
                    if (request.situacion >= 0)
                    {
                        query = query.Where(p => (int)p.Situacion == request.situacion);
                        situ = true;
                    }
                    
                    if (request.regionId >= 0)
                    {
                        if (!situ) { query = query.Where(p => p.IdRegionCaptura == request.regionId); reg = true; }
                        else
                            query = query.Where(p => p.IdRegionCaptura == request.regionId && (int)p.Situacion == request.situacion);
                    }

                    if (request.cuerpoId != "TODOS")
                    {
                        if (!situ && !reg)
                            query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId);
                        else if (situ && reg)
                            query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId && p.IdRegionCaptura == request.regionId && (int)p.Situacion == request.situacion);
                        else if (!situ && reg)
                            query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId && p.IdRegionCaptura == request.regionId );
                        else if (situ && !reg)
                            query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId && (int)p.Situacion == request.situacion);
                    }

                    

                }


                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO
                else if (request.perfilId == (int)TipoRoles.Gerente || request.perfilId == (int)TipoRoles.AtencionRegistro)
                {
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    // opciones: ver todo, o ver solo una region o gerencia y situación 
                    // si situación < 0 cualquier situación
                    bool situ = false;
                    bool reg = false;

                    if (request.situacion >= 0)
                    {
                        query = query.Where(p => (int)p.Situacion == request.situacion && p.IdCuerpoCaptura == request.cuerpoId);
                        situ = true;
                    }

                    if (request.regionId >= 0)
                    {
                        if (!situ) { query = query.Where(p => p.IdRegionCaptura == request.regionId && p.IdCuerpoCaptura == request.cuerpoId); reg = true; }
                        else
                            query = query.Where(p => p.IdRegionCaptura == request.regionId && (int)p.Situacion == request.situacion && p.IdCuerpoCaptura == request.cuerpoId);
                    }

                    if (!situ && !reg)
                        query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId);

                }
                
                //ESTOS PERFILES DE FORMA OBLIGATORIA LLEVAN EL CUERPO Y REGION
                else if (request.perfilId == (int)TipoRoles.Capturista)
                {
                    if (request.situacion >= 0)
                        query = query.Where(p => (int)p.Situacion == request.situacion && p.IdCuerpoCaptura == request.cuerpoId && p.IdRegionCaptura == request.regionId);
                    else
                        query = query.Where(p => p.IdCuerpoCaptura == request.cuerpoId && p.IdRegionCaptura == request.regionId);
                }
                else
                {
                    query = query.Where(p => p.Curp == "X");
                }
                List<Aspirante> entidades = query.ToList();

                return Result<List<API.UnidadEmpleo.Domain.Aspirante>>.Success(entidades);
            }
        }
    }


    public class GetAspirante
    {
        public class Query : IRequest<Result<Aspirante>>
        {
            public string Curp { get; set; }
            public string cuerpoId { get; set; }
            public int perfil { get; set; }
            public int region { get; set; }
        }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory, IMapper mapper, ICorporacionContextAccessor accessor, IHttpContextAccessor httpContextAccessor) : IRequestHandler<Query, Result<Aspirante>>
        {
            public async Task<Result<Aspirante>> Handle(Query request, CancellationToken cancellationToken)
            {

                //var gerente = API.Seguridad.Infrastructure.AuthContext.GetTipoRol(httpContextAccessor?.HttpContext.User);
                //string? cuerpoId = accessor.CuerpoId ?? throw new Exception("CuerpoId no proporcionado.");
                
                //Verificar si es usuario con perfil de gerente o region si es gerente ve todo abajo, si es region solo su región.
                Aspirante? aspirante;
                if (request.perfil == (int)TipoRoles.Gerente || request.perfil == (int)TipoRoles.Administrador || request.perfil == (int)TipoRoles.Subdirector)
                {
                    await using var dbContext = await _factory.CreateAsync();
                    var baseQuery = dbContext.Set<Aspirante>().AsNoTracking()
                                .Where(x => (x.Curp == request.Curp ));
                    aspirante = await baseQuery.FirstOrDefaultAsync(cancellationToken);
                }
                else
                {
                    await using var dbContext = await _factory.CreateAsync();
                    var baseQuery = dbContext.Set<Aspirante>().AsNoTracking()
                                .Where(x => (x.Curp == request.Curp && x.IdCuerpoCaptura == request.cuerpoId && x.IdRegionCaptura == request.region));
                    aspirante = await baseQuery.FirstOrDefaultAsync(cancellationToken);
                }

                if (aspirante == null) {
                    return Result<Aspirante>.Failure("No se encontró el aspirante con CURP " + request.Curp, 404);
                }
                //return Result<Aspirante>.Failure("No se encontró el ASPIRANTE", 404);

                return Result<Aspirante>.Success(aspirante);
            }
        }
    }

    public class GetAspiranteList
    {
        public class Query : IRequest<Result<List<API.UnidadEmpleo.Domain.Aspirante>>> { }

        public class Handler(UnidadEmpleoDBContextFactoryInterface _factory) : IRequestHandler<Query, Result<List<API.UnidadEmpleo.Domain.Aspirante>>>
        {
            public async Task<Result<List<API.UnidadEmpleo.Domain.Aspirante>>> Handle(Query request, CancellationToken cancellationToken)
            {
                await using var dbContext = await _factory.CreateAsync();

                var entidades = await dbContext.Set<API.UnidadEmpleo.Domain.Aspirante>()
                    .AsNoTracking()
                    .ToListAsync(cancellationToken);

                return Result<List<API.UnidadEmpleo.Domain.Aspirante>>.Success(entidades);
            }
        }
    }

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
                    .Set< API.UnidadEmpleo.Domain.Aspirante>()
                    .AsNoTracking()
                    .AsQueryable();

                if (!string.IsNullOrEmpty(request.Nombre))
                {
                    var nombreLower = request.Nombre.ToLowerInvariant();
                    query = query.Where(p =>
                        (p.Nombre + " " + p.Apellido_Paterno+ " " + (p.Apellido_Materno ?? "")).ToLower().Contains(nombreLower)
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
    }
}
