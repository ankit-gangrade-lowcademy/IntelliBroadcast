namespace IntelliBroadcast.Web.Data.Entities;

public sealed class UserExtended
{
    public long ID { get; set; }
    public long TenantID { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; }
}
