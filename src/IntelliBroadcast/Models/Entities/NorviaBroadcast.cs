namespace IntelliBroadcast.Models.Entities;

public sealed class NorviaBroadcast
{
    public int Id { get; set; }
    public int? TenantId { get; set; }
    public required string Message { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
