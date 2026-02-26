using VerticalSliceBoilerplate.Shared;

namespace CmdSeeding.AdminSeeding;

public interface IAdminSeeder
{
    Task<Result<string>> SeedAdminAsync(CancellationToken ct = default);
}

