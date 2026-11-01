namespace IntelliBroadcast.Web.Services;

public interface ILoginService
{
    Task<LoginResult> AuthenticateAsync(string email, string password, CancellationToken cancellationToken = default);
}

public sealed record LoginResult(bool Succeeded, long UserId = 0, long TenantId = 0, int RoleId = 0, string Email = "")
{
    public const string ErrorMessage = "Invalid Login";
    public static LoginResult Failed => new(false);
}
