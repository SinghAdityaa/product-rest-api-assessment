using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public sealed class ProductService(IProductRepository repository) : IProductService
{
    public async Task<PagedResult<ProductDto>> GetAsync(int pageNumber, int pageSize, CancellationToken ct)
    {
        var (items, totalCount) = await repository.GetPagedAsync(pageNumber, pageSize, ct);
        return new PagedResult<ProductDto>(items.Select(Map).ToArray(), pageNumber, pageSize, totalCount);
    }

    public async Task<ProductDto> GetByIdAsync(int id, CancellationToken ct)
        => Map(await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException($"Product {id} was not found."));

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, string actor, CancellationToken ct)
    {
        var product = new Product { ProductName = request.ProductName.Trim(), CreatedBy = actor, CreatedOn = DateTime.UtcNow };
        await repository.AddAsync(product, ct);
        await repository.SaveChangesAsync(ct);
        return Map(product);
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request, string actor, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException($"Product {id} was not found.");
        product.ProductName = request.ProductName.Trim();
        product.ModifiedBy = actor;
        product.ModifiedOn = DateTime.UtcNow;
        await repository.SaveChangesAsync(ct);
        return Map(product);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var product = await repository.GetByIdAsync(id, ct) ?? throw new NotFoundException($"Product {id} was not found.");
        repository.Remove(product);
        await repository.SaveChangesAsync(ct);
    }

    private static ProductDto Map(Product p) => new(p.Id, p.ProductName, p.CreatedBy, p.CreatedOn, p.ModifiedBy, p.ModifiedOn);
}
