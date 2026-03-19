namespace TestApi.Application.Orders;

public sealed class CreateOrderRequest
{
    public string CustomerName { get; init; } = string.Empty;
    public decimal Total { get; init; }
}
