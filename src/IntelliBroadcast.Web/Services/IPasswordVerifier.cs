using IntelliBroadcast.Web.Data.Entities;

namespace IntelliBroadcast.Web.Services;

public interface IPasswordVerifier
{
    bool Verify(UserExtended user, string password);
}
