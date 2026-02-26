using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace VerticalSliceBoilerplate.Infrastructure.Data.Postgres;

/// <summary>
/// Application DbContext with ASP.NET Core Identity. Use EF Core migrations to create/update schema:
/// dotnet ef migrations add InitialIdentity --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api
/// dotnet ef database update --project src/VerticalSliceBoilerplate.Infrastructure.Data.Postgres --startup-project src/VerticalSliceBoilerplate.Api
/// </summary>
public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<IdentityUser, IdentityRole, string>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Example: enable PostgreSQL extensions or configure common conventions here.
        // modelBuilder.HasPostgresExtension("pg_trgm");
    }
}

