namespace TestApi.Application.Orders;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<OrderDto?> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
