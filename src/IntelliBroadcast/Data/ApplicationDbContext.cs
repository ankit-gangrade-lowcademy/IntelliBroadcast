using IntelliBroadcast.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace IntelliBroadcast.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserExtended> Users => Set<UserExtended>();
    public DbSet<NorviaBroadcast> Broadcasts => Set<NorviaBroadcast>();
    public DbSet<NorviaProduct> Products => Set<NorviaProduct>();
    public DbSet<ProductUserRole> ProductRoles => Set<ProductUserRole>();
    public DbSet<UserExtendedRoleMapping> UserRoleMappings => Set<UserExtendedRoleMapping>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserExtended>(entity =>
        {
            entity.ToTable("UserExtended", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID");
            entity.Property(x => x.TenantId).HasColumnName("Tenant_ID");
            entity.Property(x => x.Name).HasMaxLength(150);
            entity.Property(x => x.Surname).HasMaxLength(150);
            entity.Property(x => x.Email).HasMaxLength(320);
            entity.Property(x => x.PasswordHash).HasMaxLength(256);
            entity.HasIndex(x => x.Email).IsUnique();
        });
        modelBuilder.Entity<NorviaBroadcast>(entity =>
        {
            entity.ToTable("NorviaBroadcast", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.Property(x => x.TenantId).HasColumnName("TenantID");
            entity.Property(x => x.Message).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Description).HasColumnType("nvarchar(max)");
            entity.Property(x => x.CreatedDate).HasPrecision(3);
        });
        modelBuilder.Entity<NorviaProduct>(entity =>
        {
            entity.ToTable("NorviaProducts", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.ProductHash).HasColumnName("ProductHash");
        });
        modelBuilder.Entity<ProductUserRole>(entity =>
        {
            entity.ToTable("ProductUserRole", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.Property(x => x.ProductId).HasColumnName("ProductID");
            entity.Property(x => x.ProductRoleId).HasColumnName("ProductRoleID");
            entity.Property(x => x.Name).HasMaxLength(255).IsRequired();
            entity.Property(x => x.Order).HasColumnName("Order");
        });
        modelBuilder.Entity<UserExtendedRoleMapping>(entity =>
        {
            entity.ToTable("UserExtendedRoleMapping", "dbo", table => table.ExcludeFromMigrations());
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasColumnName("ID").ValueGeneratedOnAdd();
            entity.Property(x => x.TenantId).HasColumnName("TenantID");
            entity.Property(x => x.UserExtendedId).HasColumnName("UserExtendedID");
            entity.Property(x => x.ProductId).HasColumnName("ProductID");
            entity.Property(x => x.ProductUserRoleId).HasColumnName("ProductUserRoleID");
        });
    }
}
