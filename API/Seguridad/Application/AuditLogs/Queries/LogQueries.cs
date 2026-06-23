using API.Persistence;
using API.Seguridad.Application.Core;
using API.Seguridad.DTOs.AuditLogs;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace API.Seguridad.Application.AuditLogs.Queries
{
    public class LogQueries
    {
        public class GetFilteredLogs
        {
            public class Query : IRequest<Result<PagedResult<LogEntryDto>>>
            {
                public DateTime StartDate { get; set; }
                public DateTime EndDate { get; set; }
                public string? Level { get; set; }
                public int PageNumber { get; set; } = 1;
                public int PageSize { get; set; } = 20;
                public string SortDirection { get; set; } = "desc"; // Default sort direction
                public string? SortBy { get; set; } = "TimeStamp"; // Default sort field
            }

            public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<PagedResult<LogEntryDto>>>
            {
                public async Task<Result<PagedResult<LogEntryDto>>> Handle(Query request, CancellationToken cancellationToken)
                {
                    if (request.StartDate > request.EndDate)
                    {
                        return Result<PagedResult<LogEntryDto>>.Failure("StartDate cannot be later than EndDate", 400);
                    }

                    if (request.PageNumber <= 0 || request.PageSize <= 0)
                    {
                        return Result<PagedResult<LogEntryDto>>.Failure("PageNumber and PageSize must be greater than zero", 400);
                    }

                    var validSortFields = new[] { "TimeStamp", "Level", "Message" };
                    if (!string.IsNullOrEmpty(request.SortBy) && !validSortFields.Contains(request.SortBy))
                    {
                        return Result<PagedResult<LogEntryDto>>.Failure($"Invalid SortBy field: {request.SortBy}", 400);
                    }

                    request.Level = request.Level.IsNullOrWhiteSpace() ? "Error" : request.Level.Trim();

                    var query = context.Logs.AsQueryable();

                    query = query.Where(l => l.TimeStamp >= request.StartDate);
                    query = query.Where(l => l.TimeStamp <= request.EndDate);
                    query = query.Where(l => l.Level == request.Level);
                    query = query.Where(l => l.UserId != null);

                    if (!string.IsNullOrEmpty(request.SortBy))
                    {
                        query = request.SortDirection == "desc"
                            ? query.OrderByDescending(e => EF.Property<object>(e, request.SortBy))
                            : query.OrderBy(e => EF.Property<object>(e, request.SortBy));
                    }
                    else
                    {
                        query = query.OrderByDescending(l => l.TimeStamp);
                    }

                    var totalRecords = await query.CountAsync(cancellationToken);

                    var pagedLogs = await query
                        .Skip((request.PageNumber - 1) * request.PageSize)
                        .Take(request.PageSize)
                        .ToListAsync(cancellationToken);

                    var logDtos = mapper.Map<List<LogEntryDto>>(pagedLogs);

                    return Result<PagedResult<LogEntryDto>>.Success(new PagedResult<LogEntryDto>
                    {
                        Data = logDtos,
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