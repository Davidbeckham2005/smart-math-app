using System.ComponentModel.DataAnnotations;
using NienLuan.Api.Entities;

namespace NienLuan.Api.Dtos;

public class RegisterRequest
{
    [Required]
    [StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(128, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;

    [StringLength(32)]
    public string? PhoneNumber { get; set; }

    [RegularExpression("^(admin|teacher|parent|student)$")]
    public string Role { get; set; } = UserRole.Student;
}
