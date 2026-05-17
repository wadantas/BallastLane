# Vehicle Store API

ASP.NET Core 8 Web API for managing a vehicle store inventory, built with **Clean Architecture** and **Use Case** pattern.

## Architecture

```
VehicleStore/
├── src/
│   ├── VehicleStore.Api/              # Controllers, Signatures, Responses, Validators
│   ├── VehicleStore.Application/      # Use cases and repository interfaces
│   ├── VehicleStore.Domain/           # Entities and domain exceptions
│   └── VehicleStore.Infrastructure/   # ADO.NET repositories and JWT
└── database/scripts/                  # SQL Server scripts
```

| Layer | Responsibility |
|-------|----------------|
| **Api** | HTTP, `*Signature` / `*Response`, FluentValidation, API mappers |
| **Application** | Use cases, business rules, repository contracts |
| **Domain** | Entities (`Vehicle`, `User`) |
| **Infrastructure** | ADO.NET data access, `*Record` types, domain mappers |

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server (LocalDB, Express, or full)

## Database setup

1. Update the connection string in `src/VehicleStore.Api/appsettings.json` if needed.
2. Run the scripts in order:

```sql
-- In SSMS or sqlcmd
:r database/scripts/001_CreateDatabase.sql
:r database/scripts/002_CreateTables.sql
```

Or execute each file manually against your SQL Server instance.

## Run the API

```bash
cd VehicleStore
dotnet run --project src/VehicleStore.Api
```

Swagger UI: `https://localhost:7xxx/swagger` (see launchSettings for the port).

On first run in **Development**, a default admin is created:

| Field | Value |
|-------|-------|
| Username | `admin` |
| Password | `Admin@123` |

## Authentication

1. `POST /api/auth/login` with credentials to obtain a JWT.
2. Use `Authorization: Bearer {token}` on protected endpoints.

## Endpoints

### Auth (anonymous)

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/auth/login` | Login and receive JWT |

### Vehicles (authenticated)

| Method | Route | Description |
|--------|-------|-------------|
| GET | `/api/vehicles` | List all vehicles |
| GET | `/api/vehicles/{id}` | Get vehicle by id |
| POST | `/api/vehicles` | Register vehicle |
| PUT | `/api/vehicles/{id}` | Update vehicle |
| DELETE | `/api/vehicles/{id}` | Delete vehicle |
| PATCH | `/api/vehicles/{id}/sold` | Mark vehicle as sold |

### Users (Admin only)

| Method | Route | Description |
|--------|-------|-------------|
| POST | `/api/users` | Create user |

## Example requests

**Login**

```json
POST /api/auth/login
{
  "username": "admin",
  "password": "Admin@123"
}
```

**Register vehicle**

```json
POST /api/vehicles
{
  "plateNumber": "ABC1D23",
  "document": "12345678901",
  "brand": "Toyota",
  "model": "Corolla",
  "year": 2024,
  "price": 95000.00
}
```

**Create user (Admin)**

```json
POST /api/users
{
  "username": "seller1",
  "email": "seller1@store.com",
  "password": "Seller@123",
  "role": "User"
}
```
