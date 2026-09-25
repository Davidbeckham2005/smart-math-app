# Mobile App — Tech Stack

## 1. Tổng quan

Mobile App phục vụ hai nhóm người dùng:

* **Phụ huynh**

  * Quản lý hồ sơ con
  * Xem lịch học
  * Theo dõi tiến độ học tập
  * Xem kết quả học tập
  * Nhận thông báo từ trung tâm
  * Thanh toán học phí trực tuyến

* **Học sinh**

  * Xem bài tập
  * Làm bài tập Toán tư duy
  * Làm bài kiểm tra năng lực
  * Luyện tập phản xạ với đồng hồ bấm giờ
  * Xem kết quả và tiến độ học tập

Mobile App giao tiếp với Backend thông qua REST API.

---

## 2. Tech Stack

| Thành phần           | Công nghệ                | Mục đích                                  |
| -------------------- | ------------------------ | ----------------------------------------- |
| Mobile Framework     | React Native             | Xây dựng ứng dụng Android/iOS             |
| Development Platform | Expo                     | Phát triển và build ứng dụng React Native |
| Programming Language | TypeScript               | Phát triển logic và UI có kiểu dữ liệu    |
| Navigation           | React Navigation         | Điều hướng giữa các màn hình              |
| State Management     | Zustand                  | Quản lý trạng thái ứng dụng               |
| HTTP Client          | Axios                    | Gọi REST API                              |
| Authentication       | JWT                      | Xác thực người dùng                       |
| Secure Storage       | Expo Secure Store        | Lưu thông tin nhạy cảm như access token   |
| Push Notification    | Firebase Cloud Messaging | Gửi thông báo đến thiết bị                |
| Backend              | ASP.NET Core Web API     | Cung cấp API cho mobile                   |
| ORM                  | Entity Framework Core    | Làm việc với cơ sở dữ liệu                |
| Database             | SQL Server               | Lưu trữ dữ liệu hệ thống                  |

---

## 3. Kiến trúc tổng thể

```text
┌──────────────────────────────────────────────┐
│                 MOBILE APP                   │
│                                              │
│        React Native + Expo + TypeScript      │
│                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────┐ │
│  │ Navigation │  │   Zustand  │  │ Axios  │ │
│  └────────────┘  └────────────┘  └────────┘ │
│                                              │
│  ┌────────────┐  ┌────────────┐  ┌────────┐ │
│  │ Secure     │  │    FCM     │  │ Screens│ │
│  │ Storage    │  │ Notification│ │        │ │
│  └────────────┘  └────────────┘  └────────┘ │
└──────────────────────┬───────────────────────┘
                       │
                       │ HTTPS / REST API
                       ▼
┌──────────────────────────────────────────────┐
│              ASP.NET CORE API                │
│                                              │
│  Controller → Service → Entity Framework     │
│                                              │
│  JWT Authentication / Authorization          │
└──────────────────────┬───────────────────────┘
                       │
                       ▼
┌──────────────────────────────────────────────┐
│                  SQL SERVER                  │
│                                              │
│ Users / Students / Courses / Classes         │
│ Assignments / Results / Payments / ...       │
└──────────────────────────────────────────────┘
```

---

# 4. Mobile Application Architecture

Mobile App được chia thành các layer chính:

```text
UI / Screens
     ↓
Components
     ↓
State Management
     ↓
Services
     ↓
API
     ↓
ASP.NET Core Backend
```

Ví dụ:

```text
StudentScreen
      ↓
student.store
      ↓
student.service.ts
      ↓
Axios
      ↓
GET /api/students/{id}
      ↓
ASP.NET Core
      ↓
SQL Server
```

---

# 5. React Native + Expo

## React Native

React Native được sử dụng để xây dựng ứng dụng mobile cho:

* Android
* iOS

Ứng dụng được phát triển bằng React và TypeScript.

## Expo

Expo được sử dụng để đơn giản hóa quá trình:

* Development
* Testing
* Build
* Quản lý native configuration
* Camera
* Notification
* Secure Storage
* Device APIs

---

# 6. TypeScript

Dự án sử dụng TypeScript thay vì JavaScript thuần.

Ví dụ model:

```ts
export interface Student {
    id: number;
    fullName: string;
    dateOfBirth: string;
    level: string;
}
```

Response từ API có thể được kiểm soát kiểu:

```ts
const response = await axios.get<Student[]>(
    "/api/students"
);
```

TypeScript giúp giảm lỗi khi frontend làm việc với API Backend.

---

# 7. Navigation

React Navigation quản lý các màn hình trong ứng dụng.

Ứng dụng có hai không gian chính:

```text
                         APP
                          │
                    Authentication
                          │
                     ┌────┴────┐
                     │         │
                  Parent    Student
                     │         │
          ┌──────────┼───┐   ┌─┼─────────┐
          │          │   │   │ │         │
        Home       Class Fee Home Quiz  Result
```

### Parent Navigation

```text
Parent
├── Home
├── Children
├── Schedule
├── Learning Progress
├── Assignments
├── Tuition
├── Payment
└── Notifications
```

### Student Navigation

```text
Student
├── Home
├── Assignments
├── Practice
├── Tests
├── Results
└── Progress
```

---

# 8. State Management

Sử dụng **Zustand** để quản lý trạng thái ứng dụng.

Một số trạng thái cần quản lý:

```text
Authentication
├── Current User
├── Access Token
└── Role

Parent
├── Selected Child
├── Children
└── Notifications

Student
├── Current Assignment
├── Quiz State
├── Timer
└── Result
```

Ví dụ:

```ts
interface AuthState {
    token: string | null;
    user: User | null;

    login: (
        token: string,
        user: User
    ) => void;

    logout: () => void;
}
```

---

# 9. API Communication

Mobile App giao tiếp với Backend bằng REST API.

Sử dụng Axios.

Ví dụ:

```ts
const response = await axios.get<Student[]>(
    "/api/students"
);
```

Luồng dữ liệu:

```text
React Native
      │
      │ Axios
      ▼
GET /api/students
      │
      ▼
ASP.NET Core Controller
      │
      ▼
Student Service
      │
      ▼
Entity Framework Core
      │
      ▼
SQL Server
```

---

# 10. Authentication

Hệ thống sử dụng JWT Authentication.

Luồng đăng nhập:

```text
Username + Password
        │
        ▼
ASP.NET Core API
        │
        ├── Invalid → 401
        │
        └── Valid
              │
              ▼
          JWT Token
              │
              ▼
       Mobile Secure Storage
```

Các request tiếp theo gửi token:

```http
Authorization: Bearer <access_token>
```

Backend xác thực token trước khi cho phép truy cập API.

---

# 11. Authorization

Hệ thống có các role chính:

```text
Admin
Teacher
Parent
Student
```

Backend kiểm soát quyền truy cập dựa trên role.

Ví dụ:

```csharp
[Authorize(Roles = "Admin")]
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    // ...
}
```

Mobile App chỉ hiển thị các chức năng phù hợp với role của người dùng.

---

# 12. Secure Storage

Thông tin nhạy cảm như access token không nên lưu bằng storage thông thường.

Có thể sử dụng:

```text
Expo Secure Store
```

Ví dụ:

```text
Login
  ↓
Receive JWT
  ↓
Secure Store
  ↓
App restart
  ↓
Read JWT
  ↓
Restore session
```

---

# 13. Push Notification

Sử dụng Firebase Cloud Messaging để gửi thông báo.

Kiến trúc:

```text
Admin / Teacher
       │
       ▼
ASP.NET Core
       │
       ▼
Firebase Cloud Messaging
       │
       ├──────────────┐
       ▼              ▼
   Parent A        Parent B
    Device           Device
```

Các thông báo có thể bao gồm:

* Lịch học sắp tới
* Thay đổi lịch học
* Bài tập mới
* Kết quả bài kiểm tra
* Thông báo học phí
* Thông báo từ trung tâm

---

# 14. Learning & Quiz Module

Đây là module tương tác chính dành cho học sinh.

Luồng làm bài:

```text
Assignment
    ↓
Question
    ↓
Start Timer
    ↓
Student Answer
    ↓
Stop Timer
    ↓
Submit
    ↓
Backend
    ↓
Save Result
```

Kết quả có thể lưu:

```text
StudentId
QuestionId
Answer
IsCorrect
ResponseTime
StartedAt
FinishedAt
```

---

# 15. Reaction Timer

Đối với các bài luyện phản xạ, timer được xử lý trực tiếp trên mobile để đảm bảo giao diện phản hồi nhanh.

Ví dụ:

```text
Question appears
       ↓
   Timer.start()
       ↓
Student answers
       ↓
   Timer.stop()
       ↓
Response Time = 2.37s
       ↓
Send result to API
```

Không cần gửi request lên server liên tục trong lúc đồng hồ chạy.

Chỉ gửi kết quả cuối cùng:

```json
{
    "questionId": 123,
    "answer": 15,
    "isCorrect": true,
    "responseTime": 2.37
}
```

---

# 16. Project Structure

Đề xuất cấu trúc:

```text
mobile/
│
├── src/
│   │
│   ├── screens/
│   │   ├── auth/
│   │   ├── parent/
│   │   └── student/
│   │
│   ├── components/
│   │
│   ├── navigation/
│   │
│   ├── services/
│   │   ├── api.ts
│   │   ├── auth.service.ts
│   │   ├── student.service.ts
│   │   ├── assignment.service.ts
│   │   └── payment.service.ts
│   │
│   ├── stores/
│   │   ├── auth.store.ts
│   │   ├── student.store.ts
│   │   └── parent.store.ts
│   │
│   ├── types/
│   │
│   ├── hooks/
│   │
│   └── utils/
│
├── assets/
│
├── android/
├── ios/
│
├── app.json
├── package.json
└── tsconfig.json
```

---

# 17. Backend Integration

Mobile App không truy cập trực tiếp SQL Server.

```text
                 MOBILE
                    │
                    │ REST API
                    ▼
            ASP.NET Core API
                    │
             Entity Framework
                    │
                    ▼
               SQL Server
```

Điều này giúp:

* Bảo vệ database
* Tập trung nghiệp vụ ở Backend
* Kiểm soát authentication/authorization
* Dễ thay đổi frontend
* Web và Mobile có thể dùng chung API

---

# 18. Development Tools

Các công cụ chính:

| Công cụ                      | Mục đích                  |
| ---------------------------- | ------------------------- |
| VS Code                      | Code React Native         |
| Android Studio               | Android SDK, Emulator     |
| Expo                         | Development và build      |
| Git                          | Version Control           |
| Postman                      | Test API                  |
| Swagger                      | Kiểm tra ASP.NET Core API |
| SQL Server Management Studio | Quản lý database          |
| Firebase Console             | Quản lý notification      |

---

# 19. Final Stack

```text
Mobile
├── React Native
├── Expo
├── TypeScript
├── React Navigation
├── Zustand
├── Axios
├── Expo Secure Store
└── Firebase Cloud Messaging

Backend
├── C#
├── ASP.NET Core Web API
├── Entity Framework Core
├── JWT Authentication
└── Authorization

Database
└── SQL Server
```

## Tổng quan

```text
┌────────────────────────────────────────────┐
│                MOBILE APP                  │
│                                            │
│ React Native + Expo + TypeScript           │
│                                            │
│ Navigation | Zustand | Axios               │
│ Secure Store | FCM | Quiz/Timer            │
└─────────────────────┬──────────────────────┘
                      │
                    HTTPS
                      │
                      ▼
┌────────────────────────────────────────────┐
│             ASP.NET CORE API               │
│                                            │
│ Controller                                 │
│     ↓                                      │
│ Service                                    │
│     ↓                                      │
│ Entity Framework Core                      │
│     ↓                                      │
│ SQL Server                                 │
└────────────────────────────────────────────┘
```
