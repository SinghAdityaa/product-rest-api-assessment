using Domain.Entities;

namespace Application.Interfaces;

public interface IProductRepository
{
    Task<(IReadOnlyCollection<Product> Items, int TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<Product?> GetByIdAsync(int id, CancellationToken ct);
    Task AddAsync(Product product, CancellationToken ct);
    void Remove(Product product);
    Task SaveChangesAsync(CancellationToken ct);
}
