using IntelliBroadcast.Web.Data;
using IntelliBroadcast.Web.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IntelliBroadcast.Web.Services;

public sealed class LoginService(
    IntelliBroadcastDbContext db,
    IPasswordVerifier passwordVerifier,
    IOptions<IntelliBroadcastOptions> options) : ILoginService
{
    public async Task<LoginResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(password)) return LoginResult.Failed;

        var user = await db.Users.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Email == normalizedEmail && x.IsActive, cancellationToken);
        if (user is null || !passwordVerifier.Verify(user, password)) return LoginResult.Failed;

        var assignment = await (
            from mapping in db.UserRoleMappings.AsNoTracking()
            join product in db.Products.AsNoTracking() on mapping.ProductID equals product.ID
            where mapping.UserExtendedID == user.ID
                && mapping.TenantID == user.TenantID
                && product.ProductHash == options.Value.ProjectHash
                && mapping.ProductUserRoleID > 0
            select new { RoleId = mapping.ProductUserRoleID })
            .FirstOrDefaultAsync(cancellationToken);

        return assignment is null
            ? LoginResult.Failed
            : new LoginResult(true, user.ID, user.TenantID, assignment.RoleId, user.Email!);
    }
}
