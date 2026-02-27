using Microsoft.AspNetCore.Identity;
using VerticalSliceBoilerplate.Core.Features.Auth.Errors;
using VerticalSliceBoilerplate.Core.Features.Auth.Services;
using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Login;

public sealed class LoginHandler : ILoginHandler
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IJwtTokenService _jwtTokenService;

    public LoginHandler(UserManager<IdentityUser> userManager, IJwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<Result<LoginResponse>> HandleAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);

        if (!await _userManager.CheckPasswordAsync(user, request.Password))
            return Result.Failure<LoginResponse>(AuthErrors.InvalidCredentials);

        var loginResponse = await _jwtTokenService.GenerateAsync(user, cancellationToken);
        return Result.Success(loginResponse);
    }
}
