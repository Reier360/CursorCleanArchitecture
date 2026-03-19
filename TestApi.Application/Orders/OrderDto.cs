namespace TestApi.Application.Orders;

public sealed class OrderDto
{
    public Guid Id { get; init; }
    public string CustomerName { get; init; } = string.Empty;
    public decimal Total { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
