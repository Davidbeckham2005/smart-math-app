using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NienLuan.Api.Common;
using NienLuan.Api.Data;
using NienLuan.Api.Dtos;
using NienLuan.Api.Entities;
using NienLuan.Api.Options;

namespace NienLuan.Api.Services;

public interface IAuthService
{
    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default);

    Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);

    Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
}

public class AuthService(
    ApplicationDbContext dbContext,
    ITokenService tokenService,
    IOptions<JwtOptions> jwtOptions) : IAuthService
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var role = request.Role.Trim().ToLowerInvariant();

        if (!UserRole.IsValid(role))
        {
            throw ApiExceptionFactory.BadRequest("Role không hợp lệ.");
        }

        if (await dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            throw ApiExceptionFactory.Conflict("Email đã được sử dụng.");
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = tokenService.Hash(request.Password),
            Role = role,
            PhoneNumber = request.PhoneNumber?.Trim(),
            CreatedAt = now,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return await IssueSessionAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            throw ApiExceptionFactory.Unauthorized("Email hoặc mật khẩu không đúng.");
        }

        if (!user.IsActive)
        {
            throw ApiExceptionFactory.Forbidden("Tài khoản đã bị vô hiệu hóa.");
        }

        user.LastLoginAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);

        return await IssueSessionAsync(user, cancellationToken);
    }

    public async Task<AuthResponse> RefreshAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            throw ApiExceptionFactory.BadRequest("Refresh token không hợp lệ.");
        }

        var storedToken = await dbContext.RefreshTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (storedToken is null)
        {
            throw ApiExceptionFactory.Unauthorized("Refresh token không hợp lệ.");
        }

        if (!storedToken.IsActive)
        {
            if (storedToken.ExpiresAt <= DateTimeOffset.UtcNow)
            {
                throw ApiExceptionFactory.Unauthorized("Refresh token đã hết hạn.");
            }

            // Token đã bị thu hồi: có thể đã bị tái sử dụng, vô hiệu toàn bộ phiên của user.
            await RevokeAllForUserAsync(storedToken.UserId, cancellationToken);
            throw ApiExceptionFactory.Unauthorized("Refresh token đã bị thu hồi. Vui lòng đăng nhập lại.");
        }

        if (storedToken.User is null || !storedToken.User.IsActive)
        {
            throw ApiExceptionFactory.Forbidden("Tài khoản không còn hoạt động.");
        }

        var session = tokenService.CreateTokens(storedToken.User);

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        storedToken.ReplacedByToken = session.RefreshToken;

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = storedToken.UserId,
            Token = session.RefreshToken,
            CreatedAt = DateTimeOffset.UtcNow,
            ExpiresAt = DateTimeOffset.UtcNow.AddDays(_jwtOptions.RefreshTokenDays),
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return BuildResponse(storedToken.User, session);
    }

    public async Task LogoutAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
        {
            return;
        }

        var storedToken = await dbContext.RefreshTokens
            .FirstOrDefaultAsync(t => t.Token == refreshToken, cancellationToken);

        if (storedToken is null || storedToken.RevokedAt is not null)
        {
            return;
        }

        storedToken.RevokedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        if (!BCrypt.Net.BCrypt.Verify(request.CurrentPassword, user.PasswordHash))
        {
            throw ApiExceptionFactory.BadRequest("Mật khẩu hiện tại không đúng.");
        }

        if (BCrypt.Net.BCrypt.Verify(request.NewPassword, user.PasswordHash))
        {
            throw ApiExceptionFactory.BadRequest("Mật khẩu mới không được trùng mật khẩu hiện tại.");
        }

        user.PasswordHash = tokenService.Hash(request.NewPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await RevokeAllForUserAsync(user.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var email = NormalizeEmail(request.Email);
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        // Luôn trả về token cho nhất quán response, kể cả khi email không tồn tại.
        if (user is null || !user.IsActive)
        {
            var placeholderExpiresAt = DateTimeOffset.UtcNow.AddMinutes(30);
            return new ForgotPasswordResponse
            {
                ResetToken = tokenService.CreatePasswordResetToken(),
                ExpiresAt = placeholderExpiresAt,
            };
        }

        var existing = await dbContext.PasswordResetTokens
            .Where(t => t.UserId == user.Id && t.UsedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in existing)
        {
            token.UsedAt = DateTimeOffset.UtcNow;
        }

        var now = DateTimeOffset.UtcNow;
        var expiresAt = now.AddMinutes(30);
        var resetToken = tokenService.CreatePasswordResetToken();

        dbContext.PasswordResetTokens.Add(new PasswordResetToken
        {
            UserId = user.Id,
            Token = resetToken,
            CreatedAt = now,
            ExpiresAt = expiresAt,
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return new ForgotPasswordResponse { ResetToken = resetToken, ExpiresAt = expiresAt };
    }

    public async Task ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        var storedToken = await dbContext.PasswordResetTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Token == request.Token, cancellationToken)
            ?? throw ApiExceptionFactory.BadRequest("Token đặt lại mật khẩu không hợp lệ.");

        if (!storedToken.IsUsable)
        {
            throw ApiExceptionFactory.BadRequest("Token đặt lại mật khẩu đã hết hạn hoặc đã sử dụng.");
        }

        if (storedToken.User is null)
        {
            throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");
        }

        storedToken.User.PasswordHash = tokenService.Hash(request.NewPassword);
        storedToken.User.UpdatedAt = DateTimeOffset.UtcNow;
        storedToken.UsedAt = DateTimeOffset.UtcNow;

        await RevokeAllForUserAsync(storedToken.UserId, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task<AuthResponse> IssueSessionAsync(User user, CancellationToken cancellationToken)
    {
        var session = tokenService.CreateTokens(user);
        var now = DateTimeOffset.UtcNow;

        dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = session.RefreshToken,
            CreatedAt = now,
            ExpiresAt = now.AddDays(_jwtOptions.RefreshTokenDays),
        });

        await dbContext.SaveChangesAsync(cancellationToken);

        return BuildResponse(user, session);
    }

    private async Task RevokeAllForUserAsync(int userId, CancellationToken cancellationToken)
    {
        var active = await dbContext.RefreshTokens
            .Where(t => t.UserId == userId && t.RevokedAt == null)
            .ToListAsync(cancellationToken);

        var now = DateTimeOffset.UtcNow;

        foreach (var token in active)
        {
            token.RevokedAt = now;
        }
    }

    private static AuthResponse BuildResponse(User user, TokenPair session) => new()
    {
        AccessToken = session.AccessToken,
        RefreshToken = session.RefreshToken,
        ExpiresAt = session.AccessTokenExpiresAt,
        User = UserResponse.FromEntity(user),
    };

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();
}
