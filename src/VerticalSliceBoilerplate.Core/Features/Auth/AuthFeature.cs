using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceBoilerplate.Core.Features.Auth.Login;
using VerticalSliceBoilerplate.Core.Features.Auth.Register;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Auth;

public static class AuthFeature
{
    public static IServiceCollection AddAuthFeature(this IServiceCollection services)
    {
        services.AddEndpointsFromNamespace(typeof(AuthFeature).Assembly, typeof(AuthFeature).Namespace!);

        services.AddScoped<IRegisterHandler, RegisterHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }
}
