using IntelliBroadcast.Web.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Web.Data;

public sealed class IntelliBroadcastDbContext(DbContextOptions<IntelliBroadcastDbContext> options) : DbContext(options)
{
    public DbSet<UserExtended> Users => Set<UserExtended>();
    public DbSet<UserExtendedRoleMapping> UserRoleMappings => Set<UserExtendedRoleMapping>();
    public DbSet<NorviaProduct> Products => Set<NorviaProduct>();
    public DbSet<NorviaBroadcast> Broadcasts => Set<NorviaBroadcast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserExtended>(entity =>
        {
            entity.ToTable("UserExtended");
            entity.HasKey(x => x.ID);
            entity.Property(x => x.ID).HasColumnName("Id");
            entity.Property(x => x.TenantID).HasColumnName("Tenant_ID");
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.PasswordHash).HasColumnName("PasswordHash").HasMaxLength(512);
            entity.HasIndex(x => x.Email);
        });
        modelBuilder.Entity<UserExtendedRoleMapping>(entity =>
        {
            entity.ToTable("UserExtendedRoleMapping");
            entity.HasKey(x => x.ID);
            entity.HasIndex(x => new { x.UserExtendedID, x.TenantID, x.ProductID });
        });
        modelBuilder.Entity<NorviaProduct>(entity =>
        {
            entity.ToTable("NorviaProducts");
            entity.HasKey(x => x.ID);
            entity.Property(x => x.Name).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(1000);
            entity.HasIndex(x => x.ProductHash).IsUnique();
        });
        modelBuilder.Entity<NorviaBroadcast>(entity =>
        {
            entity.ToTable("NorviaBroadcast");
            entity.HasKey(x => x.ID);
            entity.Property(x => x.Message).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(4000);
            entity.HasIndex(x => new { x.TenantID, x.CreatedDate });
        });
    }
}
