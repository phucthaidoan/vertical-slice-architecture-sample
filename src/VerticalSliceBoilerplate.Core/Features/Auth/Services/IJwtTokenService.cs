using Microsoft.AspNetCore.Identity;
using VerticalSliceBoilerplate.Core.Features.Auth.Login;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Services;

public interface IJwtTokenService
{
    Task<LoginResponse> GenerateAsync(IdentityUser user, CancellationToken cancellationToken = default);
}
