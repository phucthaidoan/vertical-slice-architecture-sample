using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VerticalSliceBoilerplate.Shared;

namespace CmdSeeding.AdminSeeding;

public sealed class AdminSeeder : IAdminSeeder
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<AdminSeedOptions> _options;
    private readonly ILogger<AdminSeeder> _logger;

    public AdminSeeder(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<AdminSeedOptions> options,
        ILogger<AdminSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _options = options;
        _logger = logger;
    }

    public async Task<Result<string>> SeedAdminAsync(CancellationToken ct = default)
    {
        try
        {
            var path = _options.Value.AdminFilePath ?? "SeedData/admin.json";
            var baseDir = AppContext.BaseDirectory;
            var adminJsonPath = Path.Combine(baseDir, path);

            if (!File.Exists(adminJsonPath))
            {
                _logger.LogError("Admin data file not found: {Path}", adminJsonPath);
                return Result.Failure<string>(Error.Failure(
                    "AdminSeeder.AdminFileNotFound",
                    $"Admin data file not found: {adminJsonPath}"));
            }

            var json = await File.ReadAllTextAsync(adminJsonPath, ct);
            var entries = JsonSerializer.Deserialize<List<AdminSeedData>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (entries is null || entries.Count == 0)
            {
                _logger.LogError("No admin entries found in admin.json");
                return Result.Failure<string>(Error.Failure(
                    "AdminSeeder.AdminFileEmpty",
                    "No admin entries found in admin.json"));
            }

            var entry = entries[0];
            if (string.IsNullOrWhiteSpace(entry.Email) || string.IsNullOrWhiteSpace(entry.Password))
            {
                _logger.LogError("Admin entry in admin.json must have non-empty email and password.");
                return Result.Failure<string>(Error.Failure(
                    "AdminSeeder.InvalidAdminEntry",
                    "Admin entry in admin.json must have non-empty email and password."));
            }

            var email = entry.Email;
            var password = entry.Password;

            // Ensure Admin role exists
            const string adminRoleName = "Admin";
            if (!await _roleManager.RoleExistsAsync(adminRoleName))
            {
                var roleResult = await _roleManager.CreateAsync(new IdentityRole(adminRoleName));
                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create {Role} role: {Errors}", adminRoleName, errors);
                    return Result.Failure<string>(Error.Failure(
                        "AdminSeeder.RoleCreationError",
                        $"Failed to create {adminRoleName} role: {errors}"));
                }

                _logger.LogInformation("Created {Role} role for admin seeding.", adminRoleName);
            }

            // Ensure admin user exists
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is null)
            {
                var user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user, password);
                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to create admin user {Email}: {Errors}", email, errors);
                    return Result.Failure<string>(Error.Failure(
                        "AdminSeeder.UserCreationError",
                        $"Failed to create admin user {email}: {errors}"));
                }

                existing = user;
                _logger.LogInformation("Created admin user with email {Email}", email);
            }
            else
            {
                _logger.LogInformation("Admin user with email {Email} already exists", email);
            }

            // Ensure user is in Admin role
            if (!await _userManager.IsInRoleAsync(existing, adminRoleName))
            {
                var addRoleResult = await _userManager.AddToRoleAsync(existing, adminRoleName);
                if (!addRoleResult.Succeeded)
                {
                    var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                    _logger.LogError("Failed to add admin user {Email} to {Role} role: {Errors}", email, adminRoleName, errors);
                    return Result.Failure<string>(Error.Failure(
                        "AdminSeeder.AssignRoleError",
                        $"Failed to add admin user {email} to {adminRoleName} role: {errors}"));
                }

                _logger.LogInformation("Added admin user {Email} to {Role} role", email, adminRoleName);
            }

            return Result.Success(existing.Email ?? email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed admin user");
            return Result.Failure<string>(Error.Failure(
                "AdminSeeder.Error",
                $"Failed to seed admin user: {ex.Message}"));
        }
    }
}

