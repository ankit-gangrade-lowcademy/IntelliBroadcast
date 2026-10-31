using IntelliBroadcast.Data;
using IntelliBroadcast.Identity;
using IntelliBroadcast.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Services;

public interface IUserAuthenticationService
{
    Task<UserExtended?> ValidateAsync(string email, string password, CancellationToken cancellationToken = default);
}

public sealed class AuthenticationService(ApplicationDbContext db, IPasswordHasher<UserExtended> hasher, IProjectIdentity projectIdentity) : IUserAuthenticationService
{
    public async Task<UserExtended?> ValidateAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = email.Trim().ToUpperInvariant();
        var user = await (
            from candidate in db.Users
            join mapping in db.UserRoleMappings on candidate.Id equals mapping.UserExtendedId
            join product in db.Products on mapping.ProductId equals product.Id
            join role in db.ProductRoles on mapping.ProductUserRoleId equals role.Id
            where candidate.IsActive
                && candidate.Email != null
                && candidate.Email.ToUpper() == normalizedEmail
                && mapping.TenantId == candidate.TenantId
                && product.ProductHash == projectIdentity.ProjectHash
                && role.ProductId == product.Id
            select candidate).FirstOrDefaultAsync(cancellationToken);
        if (user is null || string.IsNullOrWhiteSpace(user.PasswordHash)) return null;
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);
        if (result == PasswordVerificationResult.Failed) return null;
        if (result == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = hasher.HashPassword(user, password);
            await db.SaveChangesAsync(cancellationToken);
        }
        return user;
    }
}
