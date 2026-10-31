using IntelliBroadcast.Data;
using IntelliBroadcast.Identity;
using IntelliBroadcast.Models.Entities;
using IntelliBroadcast.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Tests;

public sealed class AuthenticationServiceTests
{
    [Fact]
    public async Task RejectsValidPassword_WhenUserHasNoIntelliBroadcastRole()
    {
        await using var db = CreateDatabase();
        var user = AddUser(db);
        AddProductRole(db, user, Guid.NewGuid());
        await db.SaveChangesAsync();

        var result = await CreateService(db).ValidateAsync(user.Email!, "Valid-password-1!");

        Assert.Null(result);
    }

    [Fact]
    public async Task AcceptsValidPassword_WhenUserHasIntelliBroadcastRole()
    {
        await using var db = CreateDatabase();
        var user = AddUser(db);
        AddProductRole(db, user, ProjectIdentity.FixedProjectHash);
        await db.SaveChangesAsync();

        var result = await CreateService(db).ValidateAsync(user.Email!, "Valid-password-1!");

        Assert.Equal(user.Id, result?.Id);
    }

    [Fact]
    public async Task RejectsRoleMapping_FromAnotherTenant()
    {
        await using var db = CreateDatabase();
        var user = AddUser(db);
        AddProductRole(db, user, ProjectIdentity.FixedProjectHash, tenantId: user.TenantId + 1);
        await db.SaveChangesAsync();

        var result = await CreateService(db).ValidateAsync(user.Email!, "Valid-password-1!");

        Assert.Null(result);
    }

    private static ApplicationDbContext CreateDatabase()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ApplicationDbContext(options);
    }

    private static UserExtended AddUser(ApplicationDbContext db)
    {
        var user = new UserExtended { Id = 10, TenantId = 20, Email = "user@example.test", IsActive = true };
        user.PasswordHash = new PasswordHasher<UserExtended>().HashPassword(user, "Valid-password-1!");
        db.Users.Add(user);
        return user;
    }

    private static void AddProductRole(ApplicationDbContext db, UserExtended user, Guid productHash, long? tenantId = null)
    {
        db.Products.Add(new NorviaProduct { Id = 30, Name = "Product", ProductHash = productHash });
        db.ProductRoles.Add(new ProductUserRole { Id = 40, ProductId = 30, ProductRoleId = 1, Name = "Broadcast" });
        db.UserRoleMappings.Add(new UserExtendedRoleMapping
        {
            Id = 50,
            TenantId = tenantId ?? user.TenantId,
            UserExtendedId = user.Id,
            ProductId = 30,
            ProductUserRoleId = 40
        });
    }

    private static AuthenticationService CreateService(ApplicationDbContext db) =>
        new(db, new PasswordHasher<UserExtended>(), new ProjectIdentity());
}
