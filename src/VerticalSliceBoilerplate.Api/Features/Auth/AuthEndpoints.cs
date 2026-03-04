using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Api.Features.Auth;

public static class AuthEndpoints
{
    public static IServiceCollection AddAuthEndpoints(this IServiceCollection services)
    {
        services.AddEndpointsFromNamespace(
            typeof(AuthEndpoints).Assembly,
            typeof(AuthEndpoints).Namespace!);
        return services;
    }
}
