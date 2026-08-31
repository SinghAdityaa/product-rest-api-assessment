using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext db, IPasswordHasher<AppUser> hasher, CancellationToken ct = default)
    {
        await db.Database.EnsureCreatedAsync(ct);
        if (!await db.Users.AnyAsync(ct))
        {
            var admin = new AppUser { Username = "admin", Role = "Admin" };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
            db.Users.Add(admin);
        }

        if (!await db.Products.AnyAsync(ct))
        {
            db.Products.AddRange(
                new Product { ProductName = "Laptop", CreatedBy = "seed", CreatedOn = DateTime.UtcNow },
                new Product { ProductName = "Keyboard", CreatedBy = "seed", CreatedOn = DateTime.UtcNow });
        }
        await db.SaveChangesAsync(ct);
    }
}
