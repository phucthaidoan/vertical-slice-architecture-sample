namespace VerticalSliceBoilerplate.Core.Features.Auth.Endpoints;

public sealed class RegisterRequest
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string? UserName { get; init; }
}
