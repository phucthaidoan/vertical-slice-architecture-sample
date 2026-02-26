using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VerticalSliceBoilerplate.Shared;

namespace CmdSeeding.RoleSeeding;

public sealed class RoleSeeder : IRoleSeeder
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IOptions<RoleSeedOptions> _options;
    private readonly ILogger<RoleSeeder> _logger;

    public RoleSeeder(
        RoleManager<IdentityRole> roleManager,
        IOptions<RoleSeedOptions> options,
        ILogger<RoleSeeder> logger)
    {
        _roleManager = roleManager;
        _options = options;
        _logger = logger;
    }

    public async Task<Result<int>> SeedRolesAsync(CancellationToken ct = default)
    {
        try
        {
            var path = _options.Value.RolesFilePath ?? "SeedData/roles.json";
            var baseDir = AppContext.BaseDirectory;
            var rolesJsonPath = Path.Combine(baseDir, path);

            if (!File.Exists(rolesJsonPath))
            {
                _logger.LogError("Roles data file not found: {Path}", rolesJsonPath);
                return Result.Failure<int>(Error.Failure("RoleSeeder.RolesFileNotFound", $"Roles data file not found: {rolesJsonPath}"));
            }

            var rolesJson = await File.ReadAllTextAsync(rolesJsonPath, ct);
            var rolesToSeed = JsonSerializer.Deserialize<List<RoleData>>(rolesJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (rolesToSeed is null || rolesToSeed.Count == 0)
            {
                _logger.LogError("No roles found in roles.json");
                return Result.Failure<int>(Error.Failure("RoleSeeder.RolesEmpty", "No roles found in roles.json"));
            }

            var seeded = 0;

            foreach (var roleData in rolesToSeed)
            {
                if (string.IsNullOrWhiteSpace(roleData.Name))
                {
                    _logger.LogWarning("Skipping role with empty name");
                    continue;
                }

                var exists = await _roleManager.RoleExistsAsync(roleData.Name);
                if (!exists)
                {
                    var role = new IdentityRole(roleData.Name);
                    var roleResult = await _roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        _logger.LogError("Failed to create {Role} role: {Errors}", roleData.Name, errors);
                        return Result.Failure<int>(Error.Failure("RoleSeeder.RoleCreationError", $"Failed to create {roleData.Name} role: {errors}"));
                    }
                    _logger.LogInformation("Created {Role} role", roleData.Name);
                    seeded++;
                }
                else
                {
                    _logger.LogDebug("{Role} role already exists", roleData.Name);
                }
            }

            if (seeded > 0)
                _logger.LogInformation("Successfully seeded {Count} roles", seeded);
            else
                _logger.LogInformation("All roles already exist. No seeding needed.");

            return Result.Success(seeded);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed roles");
            return Result.Failure<int>(Error.Failure("RoleSeeder.Error", $"Failed to seed roles: {ex.Message}"));
        }
    }
}
