Vertical Slice Boilerplate
==========================

This repository provides a minimal, opinionated boilerplate for building ASP.NET Core Web APIs using **Vertical Slice Architecture (VSA)**.

### Projects

- `VerticalSliceBoilerplate.Shared` – Result/Error primitives, domain event interfaces, and common abstractions.
- `VerticalSliceBoilerplate.Shared.Api` – API response envelope, result-to-HTTP mapping, endpoint discovery utilities, and validation filter.
- `VerticalSliceBoilerplate.Shared.Application` – Domain event dispatcher and handler abstractions.
- `VerticalSliceBoilerplate.Infrastructure.Data.Postgres` – Entity Framework Core `DbContext` with **ASP.NET Core Identity** (`IdentityDbContext<IdentityUser, IdentityRole, string>`) and domain event interceptor for PostgreSQL.
- `VerticalSliceBoilerplate.Core` – Feature slices and business logic.
- `VerticalSliceBoilerplate.Api` – Minimal API host that wires Identity, JWT authentication, authorization, and features.

### Identity and database

- **Identity** is configured with `IdentityUser` and `IdentityRole` (no custom user type). The API uses `AddIdentityCore` + `AddEntityFrameworkStores<AppDbContext>`. **The API does not seed roles at startup**; you must run the data-seeding CLI once to create the **Member** and **Admin** roles before using registration or admin endpoints (see *Role seeding* below).
- **Connection string**: Set `ConnectionStrings:Default` in `appsettings.json` or `appsettings.Development.json` (e.g. `Host=localhost;Port=5432;Database=vertical_slice_boilerplate;Username=postgres;Password=postgres`). The infrastructure reads this key when you call `AddPostgresInfrastructure(builder.Configuration)`.
- **JWT**: Configure `Authentication:Jwt` with `SigningKey` (at least 32 characters), `Issuer`, and `Audience`. In production, use a secure secret and HTTPS.
- **Migrations**: Create and apply the Identity schema with:
  ```bash
  dotnet ef migrations add InitialIdentity --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api
  dotnet ef database update --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api
  ```

### Role seeding (CLI)

Roles and the default admin user are **not** created by the API. Use the data-seeding CLI before exercising auth/admin endpoints:

- **Seed roles (Member, Admin)** – required for registration and admin-ping:

```bash
dotnet run --project misc/data-seeding/CmdSeeding -- roles
```

- **Seed a default admin user** (email/password come from `SeedData/admin.json` in the CmdSeeding project):

```bash
dotnet run --project misc/data-seeding/CmdSeeding -- admin
```

- The CLI reads role names from `SeedData/roles.json` in the CmdSeeding project (default: `[{"name":"Member"},{"name":"Admin"}]`). Ensure this file exists and is copied to the output (the project file includes `CopyToOutputDirectory` for it).
- The CLI reads admin credentials from `SeedData/admin.json`, which contains an array with at least one object like `[{ "email": "admin@example.com", "password": "Admin123!" }]` (only the first entry is used).
- The CLI uses the same connection string as the API: set `ConnectionStrings:Default` in `misc/data-seeding/CmdSeeding/appsettings.json` or `appsettings.Development.json`. Running the CLI (roles and, optionally, admin) is a **prerequisite** before using auth/registration or admin-only endpoints.

### Auth feature slice

The **Auth** feature provides sample registration, login, and protected endpoints:

- **POST /auth/register** (anonymous) – Register with `Email`, `Password`, and optional `UserName`. Assigns the **Member** role. Use FluentValidation and `ValidationFilter<RegisterRequest>`.
- **POST /auth/login** (anonymous) – Login with `Email` and `Password`. Returns a JWT in the response body (`Token`, `ExpiresAtUtc`). Use the token in the `Authorization: Bearer <token>` header for protected endpoints.
- **GET /auth/me** (authenticated) – Returns the current user’s id, email, username, and roles. Requires a valid JWT.
- **GET /auth/admin-ping** (Admin role) – Returns success only when the user has the **Admin** role. Use to test role-based authorization.

To test protected endpoints (e.g. in Swagger UI): after logging in, copy the `data.token` from the login response and click **Authorize**, then enter `Bearer <paste-token>`.

To test **Admin**-only endpoints: ensure an user has the Admin role (e.g. add a seed or use a tool to assign the role to a user in the `AspNetUserRoles` table), then log in with that user and use the returned JWT.

### Sample Feature

The `Sample` feature demonstrates the recommended slice and operation layout:

- `SampleFeature` – DI registration for the slice.
- `SampleTags` – OpenAPI tags.
- `Errors/SampleErrors` – Error catalog for the slice.
- `Ping` operation – Request/response DTOs, handler, endpoint, and validator.

You can copy/rename the `Sample` slice and `Ping` operation as a starting point for new features.

### Build and run

```bash
dotnet build VerticalSliceBoilerplate.slnx
dotnet run --project src/VerticalSliceBoilerplate.Api/VerticalSliceBoilerplate.Api.csproj
```

Then open Swagger at the URL shown (e.g. `https://localhost:5xxx/swagger`).


