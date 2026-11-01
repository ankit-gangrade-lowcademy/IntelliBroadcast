using IntelliBroadcast.Web.Data;
using IntelliBroadcast.Web.Data.Entities;
using IntelliBroadcast.Web.Options;
using IntelliBroadcast.Web.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace IntelliBroadcast.Tests;

public sealed class LoginServiceTests
{
    private static readonly Guid ProjectHash = Guid.Parse("4f0d7c72-9a31-4c68-b526-2e8f5a1d93b7");

    [Fact]
    public async Task Valid_credentials_and_matching_assignment_authenticates()
    {
        await using var db = CreateDatabase();
        SeedValidGraph(db);
        var result = await CreateService(db).AuthenticateAsync("person@example.com", "correct");
        Assert.True(result.Succeeded);
        Assert.Equal(7, result.UserId);
        Assert.Equal(42, result.TenantId);
        Assert.Equal(9, result.RoleId);
    }

    [Theory]
    [InlineData(FailureMode.NoMapping)]
    [InlineData(FailureMode.OtherProductHash)]
    [InlineData(FailureMode.OtherTenant)]
    [InlineData(FailureMode.InvalidRole)]
    [InlineData(FailureMode.InvalidCredentials)]
    public async Task Any_authentication_or_authorisation_failure_is_generic_and_cannot_authenticate(FailureMode mode)
    {
        await using var db = CreateDatabase();
        SeedValidGraph(db);
        var password = "correct";
        switch (mode)
        {
            case FailureMode.NoMapping: db.UserRoleMappings.RemoveRange(db.UserRoleMappings); break;
            case FailureMode.OtherProductHash: db.Products.Single().ProductHash = Guid.NewGuid(); break;
            case FailureMode.OtherTenant: db.UserRoleMappings.Single().TenantID = 99; break;
            case FailureMode.InvalidRole: db.UserRoleMappings.Single().ProductUserRoleID = 0; break;
            case FailureMode.InvalidCredentials: password = "wrong"; break;
        }
        await db.SaveChangesAsync();
        var result = await CreateService(db).AuthenticateAsync("person@example.com", password);
        Assert.False(result.Succeeded);
        Assert.Equal(0, result.UserId);
        Assert.Equal("Invalid Login", LoginResult.ErrorMessage);
    }

    private static IntelliBroadcastDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<IntelliBroadcastDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        return new IntelliBroadcastDbContext(options);
    }

    private static LoginService CreateService(IntelliBroadcastDbContext db) =>
        new(db, new TestPasswordVerifier(), Options.Create(new IntelliBroadcastOptions { ProjectHash = ProjectHash }));

    private static void SeedValidGraph(IntelliBroadcastDbContext db)
    {
        db.Users.Add(new UserExtended { ID = 7, TenantID = 42, Email = "person@example.com", PasswordHash = "stored-hash", IsActive = true });
        db.Products.Add(new NorviaProduct { ID = 3, Name = "IntelliBroadcast", ProductHash = ProjectHash });
        db.UserRoleMappings.Add(new UserExtendedRoleMapping { ID = 5, TenantID = 42, UserExtendedID = 7, ProductID = 3, ProductUserRoleID = 9 });
        db.SaveChanges();
    }

    public enum FailureMode { NoMapping, OtherProductHash, OtherTenant, InvalidRole, InvalidCredentials }
    private sealed class TestPasswordVerifier : IPasswordVerifier
    {
        public bool Verify(UserExtended user, string password) => password == "correct";
    }
}
