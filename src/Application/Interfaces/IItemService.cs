using Application.DTOs;

namespace Application.Interfaces;

public interface IItemService
{
    Task<IReadOnlyCollection<ItemDto>> GetByProductAsync(int productId, CancellationToken ct);
    Task<ItemDto> CreateAsync(int productId, CreateItemRequest request, CancellationToken ct);
    Task<ItemDto> UpdateAsync(int productId, int itemId, UpdateItemRequest request, CancellationToken ct);
    Task DeleteAsync(int productId, int itemId, CancellationToken ct);
}
