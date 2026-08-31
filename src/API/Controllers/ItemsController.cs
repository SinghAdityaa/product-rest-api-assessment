using Application.DTOs;
using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/products/{productId:int}/items")]
public sealed class ItemsController(IItemService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public Task<IReadOnlyCollection<ItemDto>> GetAll(int productId, CancellationToken ct) => service.GetByProductAsync(productId, ct);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ItemDto>> Create(int productId, CreateItemRequest request, CancellationToken ct)
    {
        var item = await service.CreateAsync(productId, request, ct);
        return Created($"api/v1/products/{productId}/items/{item.Id}", item);
    }

    [HttpPut("{itemId:int}")]
    [Authorize(Roles = "Admin")]
    public Task<ItemDto> Update(int productId, int itemId, UpdateItemRequest request, CancellationToken ct)
        => service.UpdateAsync(productId, itemId, request, ct);

    [HttpDelete("{itemId:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int productId, int itemId, CancellationToken ct)
    {
        await service.DeleteAsync(productId, itemId, ct);
        return NoContent();
    }
}
