using Microsoft.AspNetCore.Http;
using VerticalSliceBoilerplate.Core.Features.Auth.Login;
using VerticalSliceBoilerplate.Shared.Api;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Api.Features.Auth.Login;

public sealed class LoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", async (LoginRequest request, ILoginHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return ApiResults.ToApiResponse(result);
            })
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .WithTags(AuthTags.Auth)
            .WithSummary("Login with email and password")
            .WithDescription("Returns a JWT bearer token for use in the Authorization header.")
            .Produces<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();
    }
}
