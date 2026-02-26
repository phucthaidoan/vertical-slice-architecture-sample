using System.CommandLine;
using CmdSeeding.AdminSeeding;
using CmdSeeding.RoleSeeding;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using VerticalSliceBoilerplate.Infrastructure.Data.Context;

var builder = Host.CreateApplicationBuilder(args);

var baseDir = AppContext.BaseDirectory;
builder.Configuration
    .SetBasePath(baseDir)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();

builder.Logging.AddConsole();

var connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Connection string 'Default' not found.");

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<RoleSeedOptions>(opts =>
{
    opts.RolesFilePath = "SeedData/roles.json";
});
builder.Services.AddScoped<IRoleSeeder, RoleSeeder>();

builder.Services.Configure<AdminSeedOptions>(opts =>
{
    opts.AdminFilePath = "SeedData/admin.json";
});
builder.Services.AddScoped<IAdminSeeder, AdminSeeder>();

var app = builder.Build();
var loggerFactory = app.Services.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("CmdSeeding");

var root = new RootCommand("Vertical Slice Boilerplate data seeding CLI");

var rolesCmd = new Command("roles", "Seed roles (Member, Admin) from SeedData/roles.json");
rolesCmd.SetHandler(async () =>
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IRoleSeeder>();
    var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CmdSeeding");

    log.LogInformation("Starting roles seeding");
    var result = await seeder.SeedRolesAsync();
    if (result.IsSuccess)
    {
        log.LogInformation("Roles seeded successfully. Count: {Count}", result.Value);
        Environment.ExitCode = 0;
    }
    else
    {
        log.LogError("Roles seeding failed: {Code} {Description}", result.Error.Code, result.Error.Description);
        Environment.ExitCode = 1;
    }
});

var adminCmd = new Command("admin", "Seed or update a default admin user");
adminCmd.SetHandler(async () =>
{
    using var scope = app.Services.CreateScope();
    var seeder = scope.ServiceProvider.GetRequiredService<IAdminSeeder>();
    var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("CmdSeeding");

    log.LogInformation("Starting admin seeding");
    var result = await seeder.SeedAdminAsync();
    if (result.IsSuccess)
    {
        log.LogInformation("Admin user ensured for identifier: {Identifier}", result.Value);
        Environment.ExitCode = 0;
    }
    else
    {
        log.LogError("Admin seeding failed: {Code} {Description}", result.Error.Code, result.Error.Description);
        Environment.ExitCode = 1;
    }
});

root.AddCommand(rolesCmd);
root.AddCommand(adminCmd);

try
{
    if (args.Length == 0)
    {
        await root.InvokeAsync(["--help"]);
        return;
    }

    await root.InvokeAsync(args);
}
catch (Exception ex)
{
    logger.LogCritical(ex, "Seeding CLI crashed");
    Environment.ExitCode = 1;
}
