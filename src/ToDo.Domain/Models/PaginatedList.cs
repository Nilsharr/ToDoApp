namespace ToDo.Domain.Models;

public record PaginatedList<T>(int PageIndex, int PageSize, int TotalCount, IReadOnlyCollection<T> Data)
    where T : class
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}