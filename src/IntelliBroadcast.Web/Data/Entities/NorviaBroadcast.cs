namespace IntelliBroadcast.Web.Data.Entities;

public sealed class NorviaBroadcast
{
    public int ID { get; set; }
    public int TenantID { get; set; }
    public required string Message { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedDate { get; set; }
}
