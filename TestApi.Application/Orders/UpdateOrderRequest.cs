namespace TestApi.Application.Orders;

public sealed class UpdateOrderRequest
{
    public string? CustomerName { get; init; }
    public decimal? Total { get; init; }
    public Guid? UserId { get; init; }
}
