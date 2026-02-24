using TestApi.Application.Users;
using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Tests;

public sealed class UserServiceTests
{
    private sealed class InMemoryRepoStub : IUserRepository
    {
        private readonly List<User> _users = new();

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<User>>(_users.ToList());

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u => u.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
            => Task.FromResult(_users.SingleOrDefault(u =>
                string.Equals(u.Email, email, StringComparison.OrdinalIgnoreCase)));

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            _users.Add(user);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _users.RemoveAll(u => u.Id == id);
            return Task.CompletedTask;
        }
    }

    [Fact]
    public async Task CreateAsync_CreatesUser_WhenEmailIsUnique()
    {
        var repo = new InMemoryRepoStub();
        var service = new UserService(repo);

        var result = await service.CreateAsync(new CreateUserRequest
        {
            Email = "user@example.com",
            Name = "User"
        });

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result!.Email);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenEmailExists()
    {
        var repo = new InMemoryRepoStub();
        var existing = new User { Email = "dup@example.com", Name = "Existing" };
        await repo.AddAsync(existing);
        var service = new UserService(repo);

        var result = await service.CreateAsync(new CreateUserRequest
        {
            Email = "dup@example.com",
            Name = "New"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateAsync_ReturnsNull_WhenEmailInvalid()
    {
        var repo = new InMemoryRepoStub();
        var service = new UserService(repo);

        var result = await service.CreateAsync(new CreateUserRequest
        {
            Email = "not-an-email",
            Name = "User"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesName_WhenEmailUnchanged()
    {
        var repo = new InMemoryRepoStub();
        var user = new User { Email = "user@example.com", Name = "Old" };
        await repo.AddAsync(user);
        var service = new UserService(repo);

        var updated = await service.UpdateAsync(user.Id, new UpdateUserRequest
        {
            Name = "New"
        });

        Assert.NotNull(updated);
        Assert.Equal("New", updated!.Name);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenEmailTakenByAnother()
    {
        var repo = new InMemoryRepoStub();
        var user1 = new User { Email = "a@example.com", Name = "A" };
        var user2 = new User { Email = "b@example.com", Name = "B" };
        await repo.AddAsync(user1);
        await repo.AddAsync(user2);
        var service = new UserService(repo);

        var result = await service.UpdateAsync(user1.Id, new UpdateUserRequest
        {
            Email = "b@example.com"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNewEmailInvalid()
    {
        var repo = new InMemoryRepoStub();
        var user = new User { Email = "a@example.com", Name = "A" };
        await repo.AddAsync(user);
        var service = new UserService(repo);

        var result = await service.UpdateAsync(user.Id, new UpdateUserRequest
        {
            Email = "bad-email"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenUserMissing()
    {
        var repo = new InMemoryRepoStub();
        var service = new UserService(repo);

        var deleted = await service.DeleteAsync(Guid.NewGuid());

        Assert.False(deleted);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsTrue_WhenUserExists()
    {
        var repo = new InMemoryRepoStub();
        var user = new User { Email = "user@example.com", Name = "User" };
        await repo.AddAsync(user);
        var service = new UserService(repo);

        var deleted = await service.DeleteAsync(user.Id);

        Assert.True(deleted);
    }
}

