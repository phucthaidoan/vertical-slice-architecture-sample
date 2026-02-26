using Microsoft.AspNetCore.Identity;
using VerticalSliceBoilerplate.Core.Features.Auth.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Services;

public interface IJwtTokenService
{
    Task<LoginResponse> GenerateAsync(IdentityUser user, CancellationToken cancellationToken = default);
}
