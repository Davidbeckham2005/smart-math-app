using Microsoft.EntityFrameworkCore;
using NienLuan.Api.Common;
using NienLuan.Api.Data;
using NienLuan.Api.Dtos;
using NienLuan.Api.Entities;

namespace NienLuan.Api.Services;

public interface IUserService
{
    Task<PagedResult<UserResponse>> GetPagedAsync(int page, int pageSize, string? role, string? search, CancellationToken cancellationToken = default);

    Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<UserResponse> GetProfileAsync(int userId, CancellationToken cancellationToken = default);

    Task<UserResponse> CreateAsync(RegisterRequest request, CancellationToken cancellationToken = default);

    Task<UserResponse> UpdateAsync(int id, UpdateUserRequest request, int currentUserId, CancellationToken cancellationToken = default);

    Task DeleteAsync(int id, int currentUserId, CancellationToken cancellationToken = default);

    Task SetActiveAsync(int id, bool isActive, int currentUserId, CancellationToken cancellationToken = default);

    Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default);
}

public class UserService(ApplicationDbContext dbContext, ITokenService tokenService) : IUserService
{
    public async Task<PagedResult<UserResponse>> GetPagedAsync(
        int page,
        int pageSize,
        string? role,
        string? search,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var query = dbContext.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(role))
        {
            var normalizedRole = role.Trim().ToLowerInvariant();

            if (!UserRole.IsValid(normalizedRole))
            {
                throw ApiExceptionFactory.BadRequest("Role không hợp lệ.");
            }

            query = query.Where(u => u.Role == normalizedRole);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var keyword = $"%{search.Trim()}%";
            query = query.Where(u => EF.Functions.Like(u.FullName, keyword) || EF.Functions.Like(u.Email, keyword));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
            .OrderBy(u => u.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<UserResponse>
        {
            Items = users.Select(UserResponse.FromEntity).ToList(),
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
        };
    }

    public async Task<UserResponse> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        return UserResponse.FromEntity(user);
    }

    public Task<UserResponse> GetProfileAsync(int userId, CancellationToken cancellationToken = default) => GetByIdAsync(userId, cancellationToken);

    public async Task<UserResponse> CreateAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim().ToLowerInvariant();
        var role = request.Role.Trim().ToLowerInvariant();

        if (!UserRole.IsValid(role))
        {
            throw ApiExceptionFactory.BadRequest("Role không hợp lệ.");
        }

        if (await dbContext.Users.AnyAsync(u => u.Email == email, cancellationToken))
        {
            throw ApiExceptionFactory.Conflict("Email đã được sử dụng.");
        }

        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PasswordHash = tokenService.Hash(request.Password),
            Role = role,
            PhoneNumber = request.PhoneNumber?.Trim(),
            CreatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }

    public async Task<UserResponse> UpdateAsync(
        int id,
        UpdateUserRequest request,
        int currentUserId,
        CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        if (request.FullName is not null)
        {
            var fullName = request.FullName.Trim();

            if (string.IsNullOrWhiteSpace(fullName))
            {
                throw ApiExceptionFactory.BadRequest("Họ tên không được để trống.");
            }

            user.FullName = fullName;
        }

        if (request.Email is not null)
        {
            var email = request.Email.Trim().ToLowerInvariant();

            if (await dbContext.Users.AnyAsync(u => u.Email == email && u.Id != id, cancellationToken))
            {
                throw ApiExceptionFactory.Conflict("Email đã được sử dụng.");
            }

            user.Email = email;
        }

        if (request.Role is not null)
        {
            var role = request.Role.Trim().ToLowerInvariant();

            if (!UserRole.IsValid(role))
            {
                throw ApiExceptionFactory.BadRequest("Role không hợp lệ.");
            }

            if (id == currentUserId && role != UserRole.Admin)
            {
                throw ApiExceptionFactory.BadRequest("Không thể tự hạ quyền tài khoản đang đăng nhập.");
            }

            user.Role = role;
        }

        if (request.PhoneNumber is not null)
        {
            user.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber) ? null : request.PhoneNumber.Trim();
        }

        if (request.IsActive is not null && request.IsActive != user.IsActive)
        {
            if (id == currentUserId && !request.IsActive.Value)
            {
                throw ApiExceptionFactory.BadRequest("Không thể tự vô hiệu hóa tài khoản đang đăng nhập.");
            }

            user.IsActive = request.IsActive.Value;

            if (!user.IsActive)
            {
                await RevokeAllForUserAsync(user.Id, cancellationToken);
            }
        }

        user.UpdatedAt = DateTimeOffset.UtcNow;

        await dbContext.SaveChangesAsync(cancellationToken);

        return UserResponse.FromEntity(user);
    }

    public async Task DeleteAsync(int id, int currentUserId, CancellationToken cancellationToken = default)
    {
        if (id == currentUserId)
        {
            throw ApiExceptionFactory.BadRequest("Không thể xóa tài khoản đang đăng nhập.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task SetActiveAsync(int id, bool isActive, int currentUserId, CancellationToken cancellationToken = default)
    {
        if (id == currentUserId && !isActive)
        {
            throw ApiExceptionFactory.BadRequest("Không thể tự vô hiệu hóa tài khoản đang đăng nhập.");
        }

        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        if (user.IsActive == isActive)
        {
            return;
        }

        user.IsActive = isActive;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        if (!isActive)
        {
            await RevokeAllForUserAsync(user.Id, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ResetPasswordAsync(int id, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken)
            ?? throw ApiExceptionFactory.NotFound("Không tìm thấy tài khoản.");

        user.PasswordHash = tokenService.Hash(newPassword);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        await RevokeAllForUserAsync(user.Id, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
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
}
