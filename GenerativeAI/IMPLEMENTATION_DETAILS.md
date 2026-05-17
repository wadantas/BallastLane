# Task Management API — Implementation Details

## Overview

ASP.NET Core 8 Web API for task management with **Clean Architecture**, **use-case-driven** application logic, **ADO.NET** data access to SQL Server, **JWT authentication**, and **FluentValidation** on API contracts.

**Solution path:** `TaskManagement/TaskManagement.sln`

---

## Solution Structure

| Project | Responsibility |
|---------|----------------|
| `TaskManagement.Domain` | Entities, enums, domain exceptions — no external dependencies |
| `TaskManagement.Application` | Use cases, repository/service interfaces (ports) |
| `TaskManagement.Infrastructure` | ADO.NET repositories, JWT, password hashing, mappers |
| `TaskManagement.Api` | HTTP layer: controllers, signatures/responses, validators, middleware |

**Dependency rule:** Domain ← Application ← Infrastructure ← Api

---

## Domain Model

### Entities

- **`TaskItem`** — `Id`, `Title`, `Description`, `Status`, `DueDate`, `UserId`, optional `AssignedUser`, `CreatedAt`, `UpdatedAt`
- **`User`** — `Id`, `Name`, `Username`, `PasswordHash`, `CreatedAt`

### Enum

- **`TaskItemStatus`**: `Pending`, `InProgress`, `Completed`, `Cancelled`  
  (Named `TaskItemStatus` to avoid clash with `System.Threading.Tasks.TaskStatus`.)

### Exceptions

| Exception | HTTP mapping |
|-----------|--------------|
| `NotFoundException` | 404 |
| `ConflictException` | 409 |
| `BusinessException` | 400 |

Handled centrally by `ExceptionHandlingMiddleware`.

---

## Application Layer (Use Cases)

Each use case is a single class with `ExecuteAsync`, following SOLID (one reason to change per operation).

| Use Case | Input / Params | Output |
|----------|----------------|--------|
| `LoginUseCase` | `LoginInput` | `LoginOutput` (JWT + user info) |
| `CreateTaskUseCase` | `CreateTaskInput` | `CreateTaskOutput` (new Id) |
| `GetAllTasksUseCase` | — | `IReadOnlyList<TaskItem>` |
| `GetTaskByIdUseCase` | `Guid id` | `TaskItem` |
| `UpdateTaskUseCase` | `UpdateTaskInput` | void |
| `DeleteTaskUseCase` | `Guid id` | void |

**Repository ports (interfaces):**

- `ITaskRepository` — CRUD + `ExistsAsync`
- `IUserRepository` — `GetById`, `GetByUsername`, `Exists`
- `ITokenService` — JWT generation
- `IPasswordHasher` — PBKDF2 hash/verify

Registered via `Application.DependencyInjection.AddApplication()`.

---

## Infrastructure Layer

### ADO.NET Data Access

- **`ISqlConnectionFactory` / `SqlConnectionFactory`** — creates `SqlConnection` from `Database:ConnectionString`
- **`TaskRepository`** — parameterized SQL (`SqlCommand`) for all task operations; joins `Users` to load assignee name
- **`UserRepository`** — user lookup and existence checks

### Infra ↔ Domain Mapping

Infrastructure uses internal **record models** (`TaskRecord`, `UserRecord`) and static mappers:

- `TaskMapper.ToDomain` / `ToRecord`
- `UserMapper.ToDomain`

This keeps SQL column shapes out of the domain and application layers.

### Security

- **`PasswordHasher`** — PBKDF2-SHA256, 100k iterations, random 16-byte salt (format: `{salt}.{hash}` base64)
- **`JwtTokenService`** — HMAC-SHA256 signed JWT with claims: `NameIdentifier` (user id), `Name` (username), `name` (display name)

Configuration sections: `Database`, `Jwt` (see `appsettings.json`).

---

## API Layer

### Signature / Response Pattern

Request DTOs use the **`Signature`** suffix; response DTOs use **`Response`**:

| Signature (request) | Response |
|---------------------|----------|
| `LoginSignature` | `LoginResponse` |
| `CreateTaskSignature` | `CreateTaskResponse` |
| `UpdateTaskSignature` | — (204 No Content) |
| — | `TaskResponse`, `UserSummaryResponse`, `ErrorResponse` |

Mapping from use-case outputs to responses: `Api.Mapping.ApiMappingExtensions`.

### FluentValidation

Validators in `TaskManagement.Api.Validators`:

- `LoginSignatureValidator`
- `CreateTaskSignatureValidator`
- `UpdateTaskSignatureValidator`

Registered with `AddFluentValidationAutoValidation()` — invalid requests return **400** with validation errors before hitting use cases.

### Controllers & Authorization

| Endpoint | Auth |
|----------|------|
| `POST /api/auth/login` | Anonymous |
| `GET/POST/PUT/DELETE /api/tasks` | **Bearer JWT required** |

### Middleware

`ExceptionHandlingMiddleware` — maps domain exceptions to JSON `ErrorResponse`.

---

## Database

Scripts under `database/scripts/` (run in order):

1. `001_CreateDatabase.sql` — creates `TaskManagementDb`
2. `002_CreateTables.sql` — `Users`, `Tasks` (FK, status check constraint, indexes)
3. `003_SeedAdminUser.sql` — default admin user

**Seed credentials:** `admin` / `Admin@123`

**Connection string** (default in `appsettings.json`):

```
Server=localhost;Database=TaskManagementDb;Trusted_Connection=True;TrustServerCertificate=True;
```

---

## API Endpoints

### Authentication

```
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "Admin@123"
}
```

Response:

```json
{
  "token": "<jwt>",
  "userId": "11111111-1111-1111-1111-111111111111",
  "name": "Administrator",
  "username": "admin"
}
```

### Tasks (require `Authorization: Bearer <token>`)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/tasks` | List all tasks (with user summary) |
| GET | `/api/tasks/{id}` | Get task by id |
| POST | `/api/tasks` | Create task |
| PUT | `/api/tasks/{id}` | Update task |
| DELETE | `/api/tasks/{id}` | Delete task |

**Create / Update body example:**

```json
{
  "title": "Review pull requests",
  "description": "Check open PRs on the repo",
  "status": "Pending",
  "dueDate": "2026-05-20T17:00:00Z",
  "userId": "11111111-1111-1111-1111-111111111111"
}
```

**Valid status values:** `Pending`, `InProgress`, `Completed`, `Cancelled`

---

## Running the Project

```bash
# 1. Run SQL scripts against your SQL Server instance
# 2. Update connection string in appsettings.json if needed

cd TaskManagement
dotnet run --project src/TaskManagement.Api
```

Swagger UI: `https://localhost:<port>/swagger` (Development only).

Use **Authorize** in Swagger with: `Bearer <your-jwt-token>`.

---

## Configuration Reference

```json
{
  "Database": {
    "ConnectionString": "..."
  },
  "Jwt": {
    "Secret": "TaskManagement-Super-Secret-Key-Min-32-Chars!!",
    "Issuer": "TaskManagement.Api",
    "Audience": "TaskManagement.Client",
    "ExpirationMinutes": 60
  }
}
```

> Change `Jwt:Secret` in production. Minimum 32 characters for HMAC-SHA256.

---

## Tools

`tools/HashGen` — small console utility to generate PBKDF2 password hashes for seeding users:

```bash
dotnet run --project tools/HashGen -- "YourPassword"
```

---

## Design Decisions (SOLID)

| Principle | How it is applied |
|-----------|-------------------|
| **S** — Single Responsibility | One use case per operation; repositories only persist data |
| **O** — Open/Closed | New behaviors via new use cases / interfaces, not modifying domain |
| **L** — Liskov Substitution | Repository implementations interchangeable via interfaces |
| **I** — Interface Segregation | Focused ports: `ITaskRepository`, `IUserRepository`, etc. |
| **D** — Dependency Inversion | Application depends on abstractions; Infrastructure implements them |

---

## File Map (key paths)

```
TaskManagement/
├── TaskManagement.sln
├── IMPLEMENTATION_DETAILS.md
├── database/scripts/
├── tools/HashGen/
└── src/
    ├── TaskManagement.Domain/
    │   ├── Entities/          TaskItem.cs, User.cs
    │   ├── Enums/             TaskItemStatus.cs
    │   └── Exceptions/
    ├── TaskManagement.Application/
    │   ├── Interfaces/
    │   ├── UseCases/          Auth/, Tasks/
    │   └── DependencyInjection.cs
    ├── TaskManagement.Infrastructure/
    │   ├── Configuration/
    │   ├── Data/              SqlConnectionFactory, Models/
    │   ├── Mappers/
    │   ├── Repositories/
    │   ├── Security/
    │   └── DependencyInjection.cs
    └── TaskManagement.Api/
        ├── Contracts/         Signatures/, Responses/
        ├── Controllers/
        ├── Validators/
        ├── Mapping/
        ├── Middleware/
        └── Program.cs
```

---

## NuGet Packages

| Package | Project |
|---------|---------|
| `Microsoft.Data.SqlClient` | Infrastructure |
| `System.IdentityModel.Tokens.Jwt` | Infrastructure |
| `Microsoft.AspNetCore.Authentication.JwtBearer` (8.0.11) | Api |
| `FluentValidation.AspNetCore` | Api |
| `Microsoft.Extensions.*` | Application, Infrastructure |

---

## What Was Not Included

- Unit/integration test project (can mirror `VehicleStore.Application.Tests` pattern)
- User registration endpoint (only seeded admin for auth demo)
- Pagination/filtering on task list
- EF Core (explicitly ADO.NET per requirements)
