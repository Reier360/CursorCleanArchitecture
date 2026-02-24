using Microsoft.AspNetCore.Mvc;
using TestApi.Application.Users;
using TestApi.Controllers;

namespace TestApi.Tests;

public sealed class UsersControllerTests
{
    private sealed class UserServiceStub : IUserService
    {
        public List<UserDto> Users { get; } = new();
        public UserDto? NextSingle { get; set; }
        public UserDto? NextCreate { get; set; }
        public UserDto? NextUpdate { get; set; }
        public bool DeleteResult { get; set; }

        public Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
            => Task.FromResult<IReadOnlyList<UserDto>>(Users.ToList());

        public Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(NextSingle);

        public Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(NextCreate);

        public Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
            => Task.FromResult(NextUpdate);

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
            => Task.FromResult(DeleteResult);
    }

    [Fact]
    public async Task GetAll_ReturnsOkWithUsers()
    {
        var stub = new UserServiceStub();
        stub.Users.Add(new UserDto { Id = Guid.NewGuid(), Email = "a@example.com", Name = "A" });
        var controller = new UsersController(stub);

        var result = await controller.GetAll(CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var users = Assert.IsAssignableFrom<IReadOnlyList<UserDto>>(ok.Value);
        Assert.Single(users);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMissing()
    {
        var stub = new UserServiceStub { NextSingle = null };
        var controller = new UsersController(stub);

        var result = await controller.GetById(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenFound()
    {
        var dto = new UserDto { Id = Guid.NewGuid(), Email = "x@example.com", Name = "X" };
        var stub = new UserServiceStub { NextSingle = dto };
        var controller = new UsersController(stub);

        var result = await controller.GetById(dto.Id, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<UserDto>(ok.Value);
        Assert.Equal(dto.Id, returned.Id);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenEmailInUse()
    {
        var stub = new UserServiceStub { NextCreate = null };
        var controller = new UsersController(stub);

        var result = await controller.Create(new CreateUserRequest(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Create_ReturnsCreated_WhenSuccess()
    {
        var dto = new UserDto { Id = Guid.NewGuid(), Email = "n@example.com", Name = "N" };
        var stub = new UserServiceStub { NextCreate = dto };
        var controller = new UsersController(stub);

        var result = await controller.Create(new CreateUserRequest(), CancellationToken.None);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returned = Assert.IsType<UserDto>(created.Value);
        Assert.Equal(dto.Id, returned.Id);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenServiceFails()
    {
        var stub = new UserServiceStub { NextUpdate = null };
        var controller = new UsersController(stub);

        var result = await controller.Update(Guid.NewGuid(), new UpdateUserRequest(), CancellationToken.None);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task Update_ReturnsOk_WhenSuccess()
    {
        var dto = new UserDto { Id = Guid.NewGuid(), Email = "u@example.com", Name = "U" };
        var stub = new UserServiceStub { NextUpdate = dto };
        var controller = new UsersController(stub);

        var result = await controller.Update(dto.Id, new UpdateUserRequest(), CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(result.Result);
        var returned = Assert.IsType<UserDto>(ok.Value);
        Assert.Equal(dto.Id, returned.Id);
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenServiceReturnsFalse()
    {
        var stub = new UserServiceStub { DeleteResult = false };
        var controller = new UsersController(stub);

        var result = await controller.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenSuccess()
    {
        var stub = new UserServiceStub { DeleteResult = true };
        var controller = new UsersController(stub);

        var result = await controller.Delete(Guid.NewGuid(), CancellationToken.None);

        Assert.IsType<NoContentResult>(result);
    }
}

