using IntelliBroadcast.Web.Data.Entities;
using Microsoft.AspNetCore.Identity;

namespace IntelliBroadcast.Web.Services;

public sealed class AspNetPasswordVerifier : IPasswordVerifier
{
    private readonly PasswordHasher<UserExtended> _hasher = new();

    public bool Verify(UserExtended user, string password)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(user.PasswordHash)) return false;
        try
        {
            return _hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
        }
        catch (FormatException)
        {
            return false;
        }
    }
}
