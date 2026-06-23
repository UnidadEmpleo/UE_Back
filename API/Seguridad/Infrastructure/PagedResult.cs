
using Microsoft.EntityFrameworkCore;

public class PagedResult<T>
   {
       public List<T> Data { get; set; }
       public int TotalRecords { get; set; }
       public int TotalPages { get; set; }
       public int PageNumber { get; set; }
       public int PageSize { get; set; }
   }

public class PaginationParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public string? SortBy { get; set; }
    public string SortDirection { get; set; } = "desc";
}

public static class IQueryableExtensions
{
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, int pageNumber, int pageSize)
    {
        return query.Skip((pageNumber - 1) * pageSize).Take(pageSize);
    }

    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var totalRecords = await query.CountAsync(cancellationToken);
        var data = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return new PagedResult<T>
        {
            Data = data,
            TotalRecords = totalRecords,
            TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}

public static class PaginationValidator
{
    public static bool IsValid(int pageNumber, int pageSize) => pageNumber > 0 && pageSize > 0;
}

