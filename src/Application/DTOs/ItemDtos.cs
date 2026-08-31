namespace Application.DTOs;

public sealed record ItemDto(int Id, int ProductId, int Quantity);
public sealed record CreateItemRequest(int Quantity);
public sealed record UpdateItemRequest(int Quantity);
