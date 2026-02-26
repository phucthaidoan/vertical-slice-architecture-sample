using VerticalSliceBoilerplate.Shared;

namespace CmdSeeding.RoleSeeding;

public interface IRoleSeeder
{
    Task<Result<int>> SeedRolesAsync(CancellationToken ct = default);
}
