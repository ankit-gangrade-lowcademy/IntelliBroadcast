namespace IntelliBroadcast.Models.Entities;

public sealed class UserExtended
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public string? Name { get; set; }
    public string? Surname { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    public string? PasswordHash { get; set; }
}
