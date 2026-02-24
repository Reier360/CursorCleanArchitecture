using System.ComponentModel.DataAnnotations;

namespace TestApi.Application.Users;

public sealed class CreateUserRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;
}

public sealed class UpdateUserRequest
{
    [EmailAddress]
    public string? Email { get; set; }
    public string? Name { get; set; }
}

