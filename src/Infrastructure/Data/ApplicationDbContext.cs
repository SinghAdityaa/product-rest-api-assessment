using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public sealed class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(e =>
        {
            e.ToTable("Product");
            e.HasKey(x => x.Id);
            e.Property(x => x.ProductName).HasMaxLength(255).IsRequired();
            e.Property(x => x.CreatedBy).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreatedOn).IsRequired();
            e.Property(x => x.ModifiedBy).HasMaxLength(100);
            e.HasIndex(x => x.ProductName);
            e.HasMany(x => x.Items).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Item>(e =>
        {
            e.ToTable("Item");
            e.HasKey(x => x.Id);
            e.Property(x => x.Quantity).IsRequired();
            e.HasIndex(x => x.ProductId);
        });

        modelBuilder.Entity<AppUser>(e =>
        {
            e.ToTable("AppUser");
            e.HasKey(x => x.Id);
            e.Property(x => x.Username).HasMaxLength(100).IsRequired();
            e.HasIndex(x => x.Username).IsUnique();
            e.Property(x => x.PasswordHash).IsRequired();
            e.Property(x => x.Role).HasMaxLength(50).IsRequired();
        });

        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshToken");
            e.HasKey(x => x.Id);
            e.Property(x => x.Token).HasMaxLength(200).IsRequired();
            e.HasIndex(x => x.Token).IsUnique();
            e.HasOne(x => x.AppUser).WithMany(x => x.RefreshTokens).HasForeignKey(x => x.AppUserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
