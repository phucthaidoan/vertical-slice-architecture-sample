using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Login;

public interface ILoginHandler
{
    Task<Result<LoginResponse>> HandleAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
