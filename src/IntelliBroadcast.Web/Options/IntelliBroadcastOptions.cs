namespace IntelliBroadcast.Web.Options;

public sealed class IntelliBroadcastOptions
{
    public Guid ProjectHash { get; init; }
    public string BaseURL { get; init; } = string.Empty;
}
