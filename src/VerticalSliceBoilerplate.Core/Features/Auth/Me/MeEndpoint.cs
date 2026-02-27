using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Me;

public sealed class MeEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/me", async (ClaimsPrincipal principal, UserManager<IdentityUser> userManager, CancellationToken ct) =>
            {
                var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var user = await userManager.FindByIdAsync(userId);
                if (user is null)
                    return Results.Json(new { error = "User not found" }, statusCode: StatusCodes.Status404NotFound);

                var roles = await userManager.GetRolesAsync(user);
                return Results.Ok(new MeResponse
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    Roles = roles.ToList()
                });
            })
            .WithTags(AuthTags.Auth)
            .WithSummary("Get current user")
            .WithDescription("Returns the authenticated user's id, email, username, and roles. Requires a valid JWT.")
            .RequireAuthorization();
    }
}
