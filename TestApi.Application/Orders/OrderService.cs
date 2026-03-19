using System.Linq;
using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Application.Orders;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;
    private readonly IUserRepository _userRepository;

    public OrderService(IOrderRepository repository, IUserRepository userRepository)
    {
        _repository = repository;
        _userRepository = userRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetAllAsync(cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<IReadOnlyList<OrderDto>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var orders = await _repository.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(MapToDto).ToList();
    }

    public async Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _repository.GetByIdAsync(id, cancellationToken);
        return order is null ? null : MapToDto(order);
    }

    public async Task<OrderDto?> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Total < 0)
        {
            return null;
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            return null;
        }

        var customerName = !string.IsNullOrWhiteSpace(request.CustomerName)
            ? request.CustomerName!.Trim()
            : user.Name;

        var order = new Order
        {
            CustomerName = customerName,
            Total = request.Total,
            UserId = user.Id,
            UserName = user.Name
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

        if (request.UserId is not null && request.UserId != Guid.Empty && request.UserId != order.UserId)
        {
            var newUser = await _userRepository.GetByIdAsync(request.UserId.Value, cancellationToken);
            if (newUser is null)
            {
                return null;
            }

            order.UserId = newUser.Id;
            order.UserName = newUser.Name;
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
            UserId = order.UserId,
            UserName = order.UserName,
            Total = order.Total,
            CreatedAtUtc = order.CreatedAtUtc
        };
}
