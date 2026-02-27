namespace VerticalSliceBoilerplate.Core.Features.Auth.Me;

public sealed class MeResponse
{
    public string UserId { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = [];
}
