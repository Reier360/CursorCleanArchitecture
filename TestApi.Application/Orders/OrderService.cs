using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Application.Orders;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto?> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName) || request.Total < 0)
        {
            return null;
        }

        var order = new Order
        {
            CustomerName = request.CustomerName.Trim(),
            Total = request.Total
        };

        await _repository.AddAsync(order, cancellationToken);
        return MapToDto(order);
    }

    public async Task<OrderDto?> UpdateAsync(Guid id, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);
        if (order is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.CustomerName))
        {
            order.CustomerName = request.CustomerName!.Trim();
        }

        if (request.Total is not null)
        {
            if (request.Total < 0)
            {
                return null;
            }

            order.Total = request.Total.Value;
        }

        await _repository.UpdateAsync(order, cancellationToken);
        return MapToDto(order);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existing = await _repository.GetByIdAsync(id, cancellationToken);
        if (existing is null)
        {
            return false;
        }

        await _repository.DeleteAsync(id, cancellationToken);
        return true;
    }

    private static OrderDto MapToDto(Order order) =>
        new()
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            Total = order.Total,
            CreatedAtUtc = order.CreatedAtUtc
        };
}
