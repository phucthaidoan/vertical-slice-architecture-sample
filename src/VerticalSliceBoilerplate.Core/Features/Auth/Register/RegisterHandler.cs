using Microsoft.AspNetCore.Identity;
using VerticalSliceBoilerplate.Core.Constants;
using VerticalSliceBoilerplate.Core.Features.Auth.Errors;
using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Register;

public sealed class RegisterHandler : IRegisterHandler
{
    private readonly UserManager<IdentityUser> _userManager;

    public RegisterHandler(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<Result<RegisterResponse>> HandleAsync(RegisterRequest request, CancellationToken cancellationToken = default)
    {
        var user = new IdentityUser
        {
            UserName = request.UserName ?? request.Email,
            Email = request.Email,
            EmailConfirmed = false
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
            return Result.Failure<RegisterResponse>(AuthErrors.RegistrationFailed);

        var addRoleResult = await _userManager.AddToRoleAsync(user, Roles.Member);
        if (!addRoleResult.Succeeded)
            return Result.Failure<RegisterResponse>(AuthErrors.RegistrationFailed);

        return Result.Success(new RegisterResponse
        {
            UserId = user.Id,
            Email = user.Email ?? request.Email
        });
    }
}
