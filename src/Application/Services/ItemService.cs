using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public sealed class ItemService(IItemRepository items, IProductRepository products) : IItemService
{
    public async Task<IReadOnlyCollection<ItemDto>> GetByProductAsync(int productId, CancellationToken ct)
    {
        _ = await products.GetByIdAsync(productId, ct) ?? throw new NotFoundException($"Product {productId} was not found.");
        return (await items.GetByProductIdAsync(productId, ct)).Select(Map).ToArray();
    }

    public async Task<ItemDto> CreateAsync(int productId, CreateItemRequest request, CancellationToken ct)
    {
        _ = await products.GetByIdAsync(productId, ct) ?? throw new NotFoundException($"Product {productId} was not found.");
        var item = new Item { ProductId = productId, Quantity = request.Quantity };
        await items.AddAsync(item, ct);
        await items.SaveChangesAsync(ct);
        return Map(item);
    }

    public async Task<ItemDto> UpdateAsync(int productId, int itemId, UpdateItemRequest request, CancellationToken ct)
    {
        var item = await items.GetByIdAsync(productId, itemId, ct) ?? throw new NotFoundException($"Item {itemId} was not found for product {productId}.");
        item.Quantity = request.Quantity;
        await items.SaveChangesAsync(ct);
        return Map(item);
    }

    public async Task DeleteAsync(int productId, int itemId, CancellationToken ct)
    {
        var item = await items.GetByIdAsync(productId, itemId, ct) ?? throw new NotFoundException($"Item {itemId} was not found for product {productId}.");
        items.Remove(item);
        await items.SaveChangesAsync(ct);
    }

    private static ItemDto Map(Item i) => new(i.Id, i.ProductId, i.Quantity);
}
