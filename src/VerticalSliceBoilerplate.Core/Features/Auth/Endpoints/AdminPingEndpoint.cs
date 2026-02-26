using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using VerticalSliceBoilerplate.Core.Constants;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Auth.Endpoints;

public sealed class AdminPingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/admin-ping", () => Results.Ok(new { ok = true, message = "Admin only." }))
            .WithTags(AuthTags.Auth)
            .WithSummary("Admin-only ping")
            .WithDescription("Returns success only when the user has the Admin role. Use for testing role-based authorization.")
            .RequireAuthorization(new AuthorizeAttribute { Roles = Roles.Admin });
    }
}
