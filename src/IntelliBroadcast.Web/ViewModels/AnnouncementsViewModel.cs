namespace IntelliBroadcast.Web.ViewModels;
public sealed class AnnouncementsViewModel
{
    public required IReadOnlyList<AnnouncementRowViewModel> Items { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public int TotalPages => Math.Max(1, (int)Math.Ceiling(TotalCount / (double)PageSize));
    public bool HasPrevious => Page > 1;
    public bool HasNext => Page < TotalPages;
}
public sealed record AnnouncementRowViewModel(int ID, string Message, string? Description, DateTime CreatedDate, bool IsGlobal);
