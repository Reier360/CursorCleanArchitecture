using System.Collections.Concurrent;
using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Infrastructure.Repositories;

public sealed class InMemoryUserRepository : IUserRepository
{
    private readonly ConcurrentDictionary<Guid, User> _users = new();

    public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var list = _users.Values.OrderBy(u => u.CreatedAtUtc).ToList();
        return Task.FromResult<IReadOnlyList<User>>(list);
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryGetValue(id, out var user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = _users.Values.FirstOrDefault(
            u => string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(user);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
    {
        _users[user.Id] = user;
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        _users.TryRemove(id, out _);
        return Task.CompletedTask;
    }
}

