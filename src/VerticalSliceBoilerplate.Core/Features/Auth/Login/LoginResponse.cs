namespace VerticalSliceBoilerplate.Core.Features.Auth.Login;

public sealed class LoginResponse
{
    public string Token { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
}
