using System.ComponentModel.DataAnnotations;

namespace NienLuan.Api.Dtos;

public class ForgotPasswordRequest
{
    [Required]
    [EmailAddress]
    [StringLength(256)]
    public string Email { get; set; } = string.Empty;
}

public class ForgotPasswordResponse
{
    public string ResetToken { get; set; } = string.Empty;

    public DateTimeOffset ExpiresAt { get; set; }
}
