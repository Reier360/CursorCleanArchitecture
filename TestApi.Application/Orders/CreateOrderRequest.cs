namespace TestApi.Application.Orders;

public sealed class CreateOrderRequest
{
    public Guid UserId { get; init; }
    public string? CustomerName { get; init; }
    public decimal Total { get; init; }
}
