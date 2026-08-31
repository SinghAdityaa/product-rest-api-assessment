using Application.DTOs;

namespace Application.Interfaces;

public interface IProductService
{
    Task<PagedResult<ProductDto>> GetAsync(int pageNumber, int pageSize, CancellationToken ct);
    Task<ProductDto> GetByIdAsync(int id, CancellationToken ct);
    Task<ProductDto> CreateAsync(CreateProductRequest request, string actor, CancellationToken ct);
    Task<ProductDto> UpdateAsync(int id, UpdateProductRequest request, string actor, CancellationToken ct);
    Task DeleteAsync(int id, CancellationToken ct);
}
