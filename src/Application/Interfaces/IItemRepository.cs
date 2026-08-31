using Domain.Entities;

namespace Application.Interfaces;

public interface IItemRepository
{
    Task<IReadOnlyCollection<Item>> GetByProductIdAsync(int productId, CancellationToken ct);
    Task<Item?> GetByIdAsync(int productId, int itemId, CancellationToken ct);
    Task AddAsync(Item item, CancellationToken ct);
    void Remove(Item item);
    Task SaveChangesAsync(CancellationToken ct);
}
