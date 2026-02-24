namespace TestApi.Application.Users;

public sealed class UserDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public DateTime CreatedAtUtc { get; init; }
}

