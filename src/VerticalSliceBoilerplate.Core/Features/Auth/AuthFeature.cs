using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceBoilerplate.Core.Features.Auth.Login;
using VerticalSliceBoilerplate.Core.Features.Auth.Register;

namespace VerticalSliceBoilerplate.Core.Features.Auth;

public static class AuthFeature
{
    public static IServiceCollection AddAuthFeatureCore(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<IRegisterHandler, RegisterHandler>();
        services.AddScoped<ILoginHandler, LoginHandler>();

        // Validators
        services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
        services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();

        return services;
    }
}
