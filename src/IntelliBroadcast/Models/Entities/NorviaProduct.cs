namespace IntelliBroadcast.Models.Entities;

public sealed class NorviaProduct
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public Guid? ProductHash { get; set; }
}
