namespace VerticalSliceBoilerplate.Core.Features.Auth.Endpoints;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
