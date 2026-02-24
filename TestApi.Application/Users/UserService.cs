using System.Net.Mail;
using TestApi.Domain.Entities;
using TestApi.Domain.Interfaces;

namespace TestApi.Application.Users;

public sealed class UserService : IUserService
{
    private readonly IUserRepository _repository;

    public UserService(IUserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<UserDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var users = await _repository.GetAllAsync(cancellationToken);
        return users.Select(MapToDto).ToList();
    }

    public async Task<UserDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        return user is null ? null : MapToDto(user);
    }

    public async Task<UserDto?> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (!IsValidEmail(request.Email))
        {
            return null;
        }

        var existing = await _repository.GetByEmailAsync(request.Email, cancellationToken);
        if (existing is not null)
        {
            return null;
        }

        var user = new User
        {
            Email = request.Email.Trim(),
            Name = request.Name.Trim()
        };

        await _repository.AddAsync(user, cancellationToken);
        return MapToDto(user);
    }

    public async Task<UserDto?> UpdateAsync(Guid id, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _repository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(request.Email) &&
            !string.Equals(request.Email.Trim(), user.Email, StringComparison.OrdinalIgnoreCase))
        {
            if (!IsValidEmail(request.Email))
            {
                return null;
            }

            var existingWithEmail = await _repository.GetByEmailAsync(request.Email, cancellationToken);
            if (existingWithEmail is not null && existingWithEmail.Id != id)
            {
                return null;
            }

            user.Email = request.Email.Trim();
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            user.Name = request.Name.Trim();
        }

        await _repository.UpdateAsync(user, cancellationToken);
        return MapToDto(user);
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

    private static bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }

        try
        {
            var addr = new MailAddress(email.Trim());
            return addr.Address.Equals(email.Trim(), StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static UserDto MapToDto(User user) =>
        new()
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            CreatedAtUtc = user.CreatedAtUtc
        };
}

