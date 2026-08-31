using Application.DTOs;
using Application.Interfaces;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[ApiVersion(1.0)]
[Route("api/v{version:apiVersion}/products")]
public sealed class ProductsController(IProductService service) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public Task<PagedResult<ProductDto>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        => service.GetAsync(Math.Max(1, pageNumber), Math.Clamp(pageSize, 1, 100), ct);

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public Task<ProductDto> GetById(int id, CancellationToken ct) => service.GetByIdAsync(id, ct);

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request, CancellationToken ct)
    {
        var product = await service.CreateAsync(request, User.Identity?.Name ?? "unknown", ct);
        return CreatedAtAction(nameof(GetById), new { id = product.Id, version = "1" }, product);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> Update(int id, UpdateProductRequest request, CancellationToken ct)
        => service.UpdateAsync(id, request, User.Identity?.Name ?? "unknown", ct);

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await service.DeleteAsync(id, ct);
        return NoContent();
    }
}
