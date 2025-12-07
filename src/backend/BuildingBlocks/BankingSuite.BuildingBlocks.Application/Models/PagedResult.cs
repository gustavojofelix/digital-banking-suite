namespace BankingSuite.BuildingBlocks.Application.Models;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount
)
{
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;

    public static PagedResult<T> Empty(int pageNumber = 1, int pageSize = 20) =>
        new([], pageNumber, pageSize, 0); // [] = C# collection expression
}
