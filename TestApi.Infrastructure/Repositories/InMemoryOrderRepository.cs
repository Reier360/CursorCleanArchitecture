using System.Collections.Concurrent;
using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Infrastructure.Repositories;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public Task<IReadOnlyList<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = _orders.Values.OrderBy(o => o.CreatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<Order>>(list);
    }

    public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }

    public Task AddAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _orders[order.Id] = order;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _orders.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}
