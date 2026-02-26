using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Errors;

public static class AuthErrors
{
    public static readonly Error RegistrationFailed =
        Error.Conflict("Auth.RegistrationFailed", "User registration failed.");

    public static readonly Error InvalidCredentials =
        Error.Unauthorized("Auth.InvalidCredentials", "Invalid email or password.");

    public static readonly Error UserNotFound =
        Error.NotFound("Auth.UserNotFound", "User not found.");
}
