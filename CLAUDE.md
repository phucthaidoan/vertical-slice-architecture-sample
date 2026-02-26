# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build and Run Commands

```bash
# Build
dotnet build VerticalSliceBoilerplate.slnx

# Run API
dotnet run --project src/VerticalSliceBoilerplate.Api/VerticalSliceBoilerplate.Api.csproj

# EF Core migrations
dotnet ef migrations add <MigrationName> --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api
dotnet ef database update --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api

# Seed roles (required before using auth endpoints)
dotnet run --project misc/data-seeding/CmdSeeding -- roles

# Seed admin user (optional)
dotnet run --project misc/data-seeding/CmdSeeding -- admin
```

## Architecture

This is a **Vertical Slice Architecture** ASP.NET Core Web API. Features are organized as vertical slices where each feature contains all layers (endpoint, handler, DTOs, errors, validators) grouped together.

### Project Structure

- **Shared** – Result/Error primitives, domain event interfaces
- **Shared.Api** – API response envelope (`ApiResponse`), result-to-HTTP mapping (`ApiResults`), endpoint discovery, `ValidationFilter`
- **Shared.Application** – Domain event dispatcher abstractions
- **Infrastructure.Data.Postgres** – EF Core `DbContext` with ASP.NET Core Identity, domain event interceptor
- **Core** – Feature slices (Auth, Sample) with business logic
- **Api** – Minimal API host wiring Identity, JWT, and features

### Feature Slice Structure

```
Features/<FeatureName>/
├── <FeatureName>Feature.cs     (DI registration via extension method)
├── <FeatureName>Tags.cs        (OpenAPI tags constant)
├── Errors/<FeatureName>Errors.cs
└── <OperationName>/
    ├── <Operation>Endpoint.cs
    ├── <Operation>Request.cs
    ├── <Operation>Response.cs
    ├── <Operation>RequestValidator.cs
    ├── I<Operation>Handler.cs
    └── <Operation>Handler.cs
```

### Key Patterns

1. **IEndpoint** – All endpoints implement `IEndpoint.MapEndpoint()`. Endpoints are auto-discovered via reflection and mapped at startup.

2. **Result<T>** – Handlers return `Result<T>` instead of throwing exceptions. `ApiResults.ToApiResponse()` maps Result to HTTP responses based on `Error.Type`.

3. **Feature Registration** – Each feature exports a static extension method (e.g., `AddSampleFeature()`) that registers handlers, validators, and discovers endpoints by namespace. Called in `Program.cs`.

4. **ValidationFilter<T>** – Applied via `.AddEndpointFilter<ValidationFilter<TRequest>>()` on endpoints. Uses FluentValidation.

5. **Error Catalog** – Errors are static fields in `Errors/<Feature>Errors.cs`. Error codes follow `FeatureName.ErrorName` convention.

### Adding a New Feature

1. Create `Core/Features/<FeatureName>/` directory
2. Create operation subdirectory with Endpoint, Handler (with interface), Request, Response, Validator
3. Create `<Feature>Feature.cs` with DI registration extension method calling `AddEndpointsFromNamespace()` and registering handlers/validators
4. Create `Errors/<Feature>Errors.cs` with static Error fields
5. Register feature in `Program.cs` via `builder.Services.Add<Feature>Feature()`

### Configuration

- **Connection string**: `ConnectionStrings:Default` in appsettings
- **JWT**: `Authentication:Jwt` section with `SigningKey` (≥32 chars), `Issuer`, `Audience`
- Identity uses standard `IdentityUser` and `IdentityRole`
