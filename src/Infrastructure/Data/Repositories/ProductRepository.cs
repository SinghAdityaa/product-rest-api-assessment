using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    public async Task<(IReadOnlyCollection<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var query = db.Products.AsNoTracking().OrderBy(x => x.Id);
        var total = await query.CountAsync(ct);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return (items, total);
    }

    public Task<Product?> GetByIdAsync(int id, CancellationToken ct) => db.Products.FirstOrDefaultAsync(x => x.Id == id, ct);
    public Task AddAsync(Product product, CancellationToken ct) => db.Products.AddAsync(product, ct).AsTask();
    public void Remove(Product product) => db.Products.Remove(product);
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
