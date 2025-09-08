# TaskManagementApi

A secure and modern Task Management API built with **ASP.NET Core** and **Entity Framework Core**.

---

## 🚀 Project Flow

### 1. Startup / Program.cs
- Configures services:
  - **DbContext** (Entity Framework Core, with SQL Server LocalDB).
  - **Authentication** (JWT).
  - **Swagger** (API docs).
- Wires up controllers for handling requests.

---

### 2. Database Layer
- `ApplicationDbContext` manages the database.
- **Entities**:
  - **User**
    - Id, Username, Email, PasswordHash.
    - Linked to their tasks.
  - **TodoTask**
    - Id, Title, Description, `TaskStatus`, `TaskPriority`, DueDate, CreatedAt.
    - Linked to a `UserId`.

---

### 3. Authentication Flow
Endpoints in `AuthController`:
- **Register**
  - User sends `Username`, `Email`, `Password`.
  - Password is hashed & stored.
- **Login**
  - User sends `UsernameOrEmail` + `Password`.
  - If valid → creates and returns a **JWT token**.
- **JWT token**
  - Used for authentication in all other endpoints.

---

### 4. Task Management Flow
Endpoints in `TasksController` (secured by `[Authorize]`).

- **Create (POST /api/tasks)**
  - Creates a task for the logged-in user.
- **Read (GET /api/tasks)**
  - Supports filters (status, priority, due date, search).
  - Supports sorting and pagination.
- **Read by Id (GET /api/tasks/{id})**
  - Returns details of one task (if owned by the user).
- **Update (PUT /api/tasks/{id})**
  - Updates an existing task.
- **Delete (DELETE /api/tasks/{id})**
  - Deletes the user’s task.

---

### 5. DTOs (Data Transfer Objects)
- **TaskCreateUpdateDto**
  - Input for creating/updating a task.
- **TaskResponse**
  - Output returned by the API.

---

### 6. Enums
- **TaskStatus**
  - `Todo`, `InProgress`, `Done`.
- **TaskPriority**
  - `Low`, `Medium`, `High`.

---

## 📌 Summary

- **Authentication**: Users register/login, receive JWT tokens.  
- **Authorization**: All task endpoints require a valid token.  
- **Database**: EF Core with LocalDB stores users and their tasks.  
- **Task CRUD**: Full task lifecycle management with filtering, sorting, and pagination.  
- **Consistency**: DTOs + Entities use the same enums.  
- **Extras**: Swagger docs included for easy testing.  

👉 End result: A **secure Task Management API** where each user can only manage their own tasks, with modern API features (JWT, EF Core, filtering, pagination, Swagger).
