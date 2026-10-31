namespace IntelliBroadcast.ViewModels;

public sealed record AnnouncementItemViewModel(int Id, string Message, string? Description, DateTime CreatedDate, bool IsGlobal);

public sealed class AnnouncementIndexViewModel
{
    public IReadOnlyList<AnnouncementItemViewModel> Items { get; init; } = [];
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
}
