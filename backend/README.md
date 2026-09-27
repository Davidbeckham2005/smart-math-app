# Backend — ASP.NET Core Web API

REST API cho mobile app (React Native/Expo). Hiện phục vụ module **tài khoản người dùng**:
đăng ký, đăng nhập JWT, refresh token, quản lý mật khẩu và quản lý người dùng (Admin).

## Tech stack

- .NET 10 / ASP.NET Core Web API
- Entity Framework Core 10 + SQL Server (MSSQLSERVER01)
- JWT Bearer Authentication
- BCrypt.Net-Next (hash mật khẩu)
- Swagger (chỉ bật ở môi trường Development)

## Cấu trúc

```text
backend/
├── NienLuan.slnx
├── dotnet-tools.json                 # dotnet-ef local tool
└── src/NienLuan.Api/
    ├── Program.cs                    # DI, JWT, EF, migration + seed lúc startup
    ├── Entities/                     # User, RefreshToken, PasswordResetToken
    ├── Data/
    │   ├── ApplicationDbContext.cs
    │   └── DbSeeder.cs
    ├── Dtos/                         # Request/Response contracts
    ├── Options/JwtOptions.cs
    ├── Services/
    │   ├── ITokenService.cs          # phát hành JWT + refresh token
    │   ├── AuthService.cs
    │   └── UserService.cs
    ├── Controllers/
    │   ├── AuthController.cs
    │   └── UsersController.cs
    └── Migrations/
```

## Yêu cầu

- .NET SDK 10.0.401+
- SQL Server instance `MSSQLSERVER01` đang chạy (Windows Authentication)

## Cấu hình

Connection string trong `src/NienLuan.Api/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.\\MSSQLSERVER01;Database=NienLuanDb;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

JWT trong `appsettings.json`, phần `SigningKey` để trống và chỉ điền ở
`appsettings.Development.json` (dev) hoặc qua biến môi trường (production):

```powershell
# Production
$env:Jwt__SigningKey = "<chuoi-ngau-nhien-toi-thieu-64-ky-tu>"
```

`Program.cs` sẽ throw khi `SigningKey` thiếu hoặc ngắn hơn 32 ký tự (Development) /
64 ký tự (Production).

## Chạy backend

Từ thư mục `backend/`:

```powershell
dotnet tool restore
dotnet run --project src\NienLuan.Api --launch-profile http
```

API chạy ở `http://localhost:5000`, Swagger ở `http://localhost:5000/swagger`.

Khi khởi động, ứng dụng tự:

1. `Database.MigrateAsync()` — tạo/cập nhật database `NienLuanDb` trên SQL Server.
2. Seed tài khoản demo nếu bảng `Users` còn trống.

### Profile khác

```powershell
dotnet run --project src\NienLuan.Api --launch-profile https   # http 5000 + https 5001
```

## Quản lý migration

```powershell
# Thêm migration
dotnet tool run dotnet-ef migrations add <Ten> `
  --project src\NienLuan.Api\NienLuan.Api.csproj `
  --startup-project src\NienLuan.Api\NienLuan.Api.csproj

# Áp dụng
dotnet tool run dotnet-ef database update `
  --project src\NienLuan.Api\NienLuan.Api.csproj `
  --startup-project src\NienLuan.Api\NienLuan.Api.csproj

# Xem các migration đã có
dotnet tool run dotnet-ef migrations list `
  --project src\NienLuan.Api\NienLuan.Api.csproj `
  --startup-project src\NienLuan.Api\NienLuan.Api.csproj
```

## Tài khoản seed

| Role     | Email                   | Mật khẩu     |
| -------- | ----------------------- | ------------ |
| admin    | `admin@nienluan.local`   | `Admin@123`  |
| teacher  | `teacher@nienluan.local` | `Teacher@123`|
| parent   | `parent@nienluan.local`  | `Parent@123` |
| student  | `student@nienluan.local` | `Student@123`|

## API

Tất cả response/error dùng `ProblemDetails` và format camelCase.

### Auth — `/api/auth`

| Method | Route                | Auth  | Mô tả                                     |
| ------ | -------------------- | ----- | ----------------------------------------- |
| POST   | `/auth/register`     | none  | Đăng ký, trả về access + refresh token    |
| POST   | `/auth/login`        | none  | Đăng nhập, trả về access + refresh token  |
| POST   | `/auth/refresh`      | none  | Làm mới access token (xoay refresh token) |
| POST   | `/auth/logout`       | none  | Thu hồi refresh token                     |
| POST   | `/auth/change-password` | JWT | Đổi mật khẩu (cần mật khẩu hiện tại)    |
| POST   | `/auth/forgot-password` | none | Sinh reset token (hết hạn sau 30 phút)  |
| POST   | `/auth/reset-password` | none | Đặt lại mật khẩu bằng reset token        |

### Users — `/api/users`

| Method | Route                          | Auth  | Mô tả                                |
| ------ | ------------------------------ | ----- | ------------------------------------ |
| GET    | `/users/me`                    | JWT   | Hồ sơ của người đang đăng nhập       |
| GET    | `/users/{id}`                  | JWT   | Chi tiết (Admin hoặc chính mình)    |
| GET    | `/users`                       | Admin | Danh sách phân trang + lọc/search   |
| POST   | `/users`                       | Admin | Tạo tài khoản                        |
| PUT    | `/users/{id}`                  | Admin | Cập nhật họ tên/email/role/trạng thái|
| PATCH  | `/users/{id}/status`           | Admin | Bật/vô hiệu hóa tài khoản            |
| POST   | `/users/{id}/reset-password`   | Admin | Đặt lại mật khẩu cho user            |
| DELETE | `/users/{id}`                  | Admin | Xóa tài khoản                        |

Query của `GET /api/users`: `page` (mặc định 1), `pageSize` (mặc định 20, tối đa 100),
`role`, `search` (khớp `fullName` hoặc `email`).

### Health

- `GET /health` → `{"status":"ok"}`

## Quy tắc bảo mật

- Mật khẩu lưu bằng BCrypt, không bao giờ trả về client.
- Access token (JWT) hết hạn sau 30 phút; refresh token sau 14 ngày.
- Mỗi lần refresh sẽ **xoay** refresh token và thu hồi token cũ.
- Dùng lại refresh token đã thu hồi sẽ thu hồi toàn bộ phiên của user đó.
- Đổi mật khẩu, reset mật khẩu, và vô hiệu hóa tài khoản đều thu hồi mọi refresh token.
- Admin không thể tự xóa, tự hạ quyền hoặc tự vô hiệu hóa tài khoản đang đăng nhập.
- `forgot-password` luôn trả `200` kể cả email không tồn tại, để không lộ thông tin user.
