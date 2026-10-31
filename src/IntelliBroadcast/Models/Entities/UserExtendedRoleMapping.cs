namespace IntelliBroadcast.Models.Entities;

public sealed class UserExtendedRoleMapping
{
    public long Id { get; set; }
    public long TenantId { get; set; }
    public long UserExtendedId { get; set; }
    public int ProductId { get; set; }
    public int ProductUserRoleId { get; set; }
}
