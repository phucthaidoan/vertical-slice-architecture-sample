using VerticalSliceBoilerplate.Shared;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Register;

public interface IRegisterHandler
{
    Task<Result<RegisterResponse>> HandleAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}
