namespace Application.DTOs;

public sealed record ProductDto(int Id, string ProductName, string CreatedBy, DateTime CreatedOn, string? ModifiedBy, DateTime? ModifiedOn);
public sealed record CreateProductRequest(string ProductName);
public sealed record UpdateProductRequest(string ProductName);
public sealed record PagedResult<T>(IReadOnlyCollection<T> Items, int PageNumber, int PageSize, int TotalCount);
