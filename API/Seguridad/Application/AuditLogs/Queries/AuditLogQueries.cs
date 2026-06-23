using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.DTOs.AuditLogs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.AuditLogs.Queries
{
    public class AuditLogQueries
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class GetAuditLogList
        {
            public class Query : IRequest<Result<List<AuditLogDto>>> { }

            public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<List<AuditLogDto>>>
            {
                public async Task<Result<List<AuditLogDto>>> Handle(Query request, CancellationToken cancellationToken)
                {
                    var auditLogs = await context.AuditLogs.ToListAsync(cancellationToken);
                    var auditLogDtos = mapper.Map<List<AuditLogDto>>(auditLogs);

                    return Result<List<AuditLogDto>>.Success(auditLogDtos);
                }
            }
        }

        public class GetAuditLogDetails
        {
            public class Query : IRequest<Result<AuditLogDto>>
            {
                public int Id { get; set; }
            }

            public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<AuditLogDto>>
            {
                public async Task<Result<AuditLogDto>> Handle(Query request, CancellationToken cancellationToken)
                {
                    var auditLog = await context.AuditLogs
                        .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                    if (auditLog == null) return Result<AuditLogDto>.Failure("Audit log not found", 404);

                    var auditLogDto = mapper.Map<AuditLogDto>(auditLog);
                    return Result<AuditLogDto>.Success(auditLogDto);
                }
            }
        }

        public class GetFilteredAuditLogs
        {
            public class Query : IRequest<Result<PagedResult<AuditLogDto>>>
            {
                public DateTime? StartDate { get; set; }
                public DateTime? EndDate { get; set; }
                public List<string>? UserIds { get; set; }
                public int PageNumber { get; set; } = 1;
                public int PageSize { get; set; } = 20;
                public string SortDirection { get; set; } = "desc"; // Default sort direction
                public string? SortBy { get; set; } = "Timestamp"; // Default sort field
            }

            public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<PagedResult<AuditLogDto>>>
            {
                public async Task<Result<PagedResult<AuditLogDto>>> Handle(Query request, CancellationToken cancellationToken)
                {
                    if (request.StartDate.HasValue && request.EndDate.HasValue && request.StartDate > request.EndDate)
                    {
                        return Result<PagedResult<AuditLogDto>>.Failure("StartDate cannot be later than EndDate", 400);
                    }

                    if (!request.StartDate.HasValue && !request.EndDate.HasValue)
                    {
                        request.StartDate = DateTime.UtcNow.AddDays(-7);
                        request.EndDate = DateTime.UtcNow;
                    }

                    if (request.PageNumber <= 0 || request.PageSize <= 0)
                    {
                        return Result<PagedResult<AuditLogDto>>.Failure("PageNumber and PageSize must be greater than zero", 400);
                    }

                    var validSortFields = new[] { "Timestamp", "UserName", "EventType" };
                    if (!string.IsNullOrEmpty(request.SortBy) && !validSortFields.Contains(request.SortBy))
                    {
                        return Result<PagedResult<AuditLogDto>>.Failure($"Invalid SortBy field: {request.SortBy}", 400);
                    }

                    var query = context.AuditLogs.AsQueryable();

                    if (request.StartDate.HasValue)
                    {
                        query = query.Where(a => a.Timestamp >= request.StartDate.Value);
                    }

                    if (request.EndDate.HasValue)
                    {
                        query = query.Where(a => a.Timestamp <= request.EndDate.Value);
                    }

                    if (request.UserIds != null && request.UserIds.Any())
                    {
                        query = query.Where(a => request.UserIds.Contains(a.UserId));
                    }

                    if (!string.IsNullOrEmpty(request.SortBy))
                    {
                        query = request.SortDirection == "desc"
                            ? query.OrderByDescending(e => EF.Property<object>(e, request.SortBy))
                            : query.OrderBy(e => EF.Property<object>(e, request.SortBy));
                    }
                    else
                    {
                        query = query.OrderByDescending(a => a.Timestamp);
                    }

                    var totalRecords = await query.CountAsync(cancellationToken);

                    var pagedAuditLogs = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(cancellationToken);

                    var auditLogDtos = mapper.Map<List<AuditLogDto>>(pagedAuditLogs);

                    return Result<PagedResult<AuditLogDto>>.Success(new PagedResult<AuditLogDto>
                    {
                        Data = auditLogDtos,
                        TotalRecords = totalRecords,
                        TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                        PageNumber = request.PageNumber,
                        PageSize = request.PageSize
                    });
                }
            }
        }
    }
}
