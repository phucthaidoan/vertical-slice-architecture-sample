namespace CmdSeeding.AdminSeeding;

internal sealed record AdminSeedData
{
    public string Email { get; init; } = string.Empty;

    public string Password { get; init; } = string.Empty;
}

