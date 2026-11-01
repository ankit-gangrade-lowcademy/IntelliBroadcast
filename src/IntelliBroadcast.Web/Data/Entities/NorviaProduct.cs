namespace IntelliBroadcast.Web.Data.Entities;

public sealed class NorviaProduct
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid ProductHash { get; set; }
}
