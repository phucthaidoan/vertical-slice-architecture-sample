using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VerticalSliceBoilerplate.Shared.Api;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Endpoints;

public sealed class RegisterEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", async (RegisterRequest request, IRegisterHandler handler, CancellationToken ct) =>
            {
                var result = await handler.HandleAsync(request, ct);
                return ApiResults.ToApiResponse(result);
            })
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>()
            .WithTags(AuthTags.Auth)
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user with email and password. User is assigned the Member role.")
            .Produces<ApiResponse<RegisterResponse>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .AllowAnonymous();
    }
}
