using System.ComponentModel.DataAnnotations;

namespace NienLuan.Api.Dtos;

public class RefreshRequest
{
    [Required]
    public string RefreshToken { get; set; } = string.Empty;
}
