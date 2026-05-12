namespace Supplements.ViewModels.Shared;

public class PagedViewModel<T>
{
    public List<T> Items { get; set; } = new();
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 12;
    public int TotalCount { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}
