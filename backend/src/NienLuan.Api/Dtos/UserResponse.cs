using System.ComponentModel.DataAnnotations;
using NienLuan.Api.Entities;

namespace NienLuan.Api.Dtos;

public class UserResponse
{
    public int Id { get; init; }

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string? PhoneNumber { get; init; }

    public bool IsActive { get; init; }

    public DateTimeOffset CreatedAt { get; init; }

    public DateTimeOffset? LastLoginAt { get; init; }

    public static UserResponse FromEntity(User user) => new()
    {
        Id = user.Id,
        FullName = user.FullName,
        Email = user.Email,
        Role = user.Role,
        PhoneNumber = user.PhoneNumber,
        IsActive = user.IsActive,
        CreatedAt = user.CreatedAt,
        LastLoginAt = user.LastLoginAt,
    };
}

public class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; init; }

    public UserResponse User { get; init; } = new();
}

public class UpdateUserRequest
{
    [StringLength(200)]
    public string? FullName { get; set; }

    [EmailAddress]
    [StringLength(256)]
    public string? Email { get; set; }

    [RegularExpression("^(admin|teacher|parent|student)$")]
    public string? Role { get; set; }

    [StringLength(32)]
    public string? PhoneNumber { get; set; }

    public bool? IsActive { get; set; }
}

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalCount { get; init; }

    public int TotalPages => PageSize <= 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}
