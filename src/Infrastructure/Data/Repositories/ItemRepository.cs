using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public sealed class ItemRepository(ApplicationDbContext db) : IItemRepository
{
    public async Task<IReadOnlyCollection<Item>> GetByProductIdAsync(int productId, CancellationToken ct)
        => await db.Items.AsNoTracking().Where(x => x.ProductId == productId).OrderBy(x => x.Id).ToListAsync(ct);

    public Task<Item?> GetByIdAsync(int productId, int itemId, CancellationToken ct)
        => db.Items.FirstOrDefaultAsync(x => x.ProductId == productId && x.Id == itemId, ct);

    public Task AddAsync(Item item, CancellationToken ct) => db.Items.AddAsync(item, ct).AsTask();
    public void Remove(Item item) => db.Items.Remove(item);
    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
