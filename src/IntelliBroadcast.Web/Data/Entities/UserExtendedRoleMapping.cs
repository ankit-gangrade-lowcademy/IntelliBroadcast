namespace IntelliBroadcast.Web.Data.Entities;

public sealed class UserExtendedRoleMapping
{
    public long ID { get; set; }
    public long TenantID { get; set; }
    public long UserExtendedID { get; set; }
    public int ProductID { get; set; }
    public int ProductUserRoleID { get; set; }
}
