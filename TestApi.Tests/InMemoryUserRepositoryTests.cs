using TestApi.Domain.Entities;
using TestApi.Infrastructure.Repositories;

namespace TestApi.Tests;

public sealed class InMemoryUserRepositoryTests
{
    [Fact]
    public async Task Add_And_GetById_Works()
    {
        var repo = new InMemoryUserRepository();
        var user = new User { Email = "test@example.com", Name = "Test" };

        await repo.AddAsync(user);
        var fetched = await repo.GetByIdAsync(user.Id);

        Assert.NotNull(fetched);
        Assert.Equal(user.Email, fetched!.Email);
    }

    [Fact]
    public async Task GetByEmail_IsCaseInsensitive()
    {
        var repo = new InMemoryUserRepository();
        var user = new User { Email = "Case@Test.com", Name = "Case" };
        await repo.AddAsync(user);

        var fetched = await repo.GetByEmailAsync("case@test.com");

        Assert.NotNull(fetched);
        Assert.Equal(user.Id, fetched!.Id);
    }

    [Fact]
    public async Task GetAll_ReturnsUsersOrderedByCreatedAt()
    {
        var repo = new InMemoryUserRepository();
        var first = new User { Email = "a@example.com", Name = "A", CreatedAtUtc = DateTime.UtcNow.AddMinutes(-1) };
        var second = new User { Email = "b@example.com", Name = "B", CreatedAtUtc = DateTime.UtcNow };

        await repo.AddAsync(second);
        await repo.AddAsync(first);

        var all = await repo.GetAllAsync();

        Assert.Collection(all,
            u => Assert.Equal(first.Id, u.Id),
            u => Assert.Equal(second.Id, u.Id));
    }

    [Fact]
    public async Task Delete_RemovesUser()
    {
        var repo = new InMemoryUserRepository();
        var user = new User { Email = "x@example.com", Name = "X" };
        await repo.AddAsync(user);

        await repo.DeleteAsync(user.Id);
        var fetched = await repo.GetByIdAsync(user.Id);

        Assert.Null(fetched);
    }
}

