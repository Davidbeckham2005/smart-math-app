using System.Security.Claims;

namespace NienLuan.Api.Common;

public static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(value, out var userId))
        {
            throw ApiExceptionFactory.Unauthorized("Token không hợp lệ.");
        }

        return userId;
    }

    public static string GetUserRole(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Role) ?? string.Empty;
}
