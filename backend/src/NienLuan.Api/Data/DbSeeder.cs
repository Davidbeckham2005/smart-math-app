using Microsoft.EntityFrameworkCore;
using NienLuan.Api.Entities;
using NienLuan.Api.Services;

namespace NienLuan.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext, ITokenService tokenService, ILogger logger)
    {
        if (await dbContext.Users.AnyAsync())
        {
            logger.LogInformation("Database already contains users, skipping seed.");
            return;
        }

        var now = DateTimeOffset.UtcNow;

        dbContext.Users.AddRange(
            new User
            {
                FullName = "Quản trị hệ thống",
                Email = "admin@nienluan.local",
                PasswordHash = tokenService.Hash("Admin@123"),
                Role = UserRole.Admin,
                PhoneNumber = "0900000001",
                CreatedAt = now,
            },
            new User
            {
                FullName = "Giáo viên chủ nhiệm",
                Email = "teacher@nienluan.local",
                PasswordHash = tokenService.Hash("Teacher@123"),
                Role = UserRole.Teacher,
                PhoneNumber = "0900000002",
                CreatedAt = now,
            },
            new User
            {
                FullName = "Nguyễn Minh Anh",
                Email = "parent@nienluan.local",
                PasswordHash = tokenService.Hash("Parent@123"),
                Role = UserRole.Parent,
                PhoneNumber = "0900000003",
                CreatedAt = now,
            },
            new User
            {
                FullName = "Trần Gia Bảo",
                Email = "student@nienluan.local",
                PasswordHash = tokenService.Hash("Student@123"),
                Role = UserRole.Student,
                PhoneNumber = "0900000004",
                CreatedAt = now,
            });

        await dbContext.SaveChangesAsync();

        logger.LogInformation("Seeded 4 demo accounts: admin/teacher/parent/student @nienluan.local.");
    }
}
