namespace NienLuan.Api.Entities;

public static class UserRole
{
    public const string Admin = "admin";
    public const string Teacher = "teacher";
    public const string Parent = "parent";
    public const string Student = "student";

    public static readonly IReadOnlyList<string> All = [Admin, Teacher, Parent, Student];

    public static bool IsValid(string? role) =>
        role is not null && All.Contains(role, StringComparer.OrdinalIgnoreCase);
}
