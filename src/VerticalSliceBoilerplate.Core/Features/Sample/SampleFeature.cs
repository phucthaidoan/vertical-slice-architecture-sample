using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceBoilerplate.Core.Features.Sample.Ping;

namespace VerticalSliceBoilerplate.Core.Features.Sample;

public static class SampleFeature
{
    public static IServiceCollection AddSampleFeatureCore(this IServiceCollection services)
    {
        // Handlers
        services.AddScoped<IPingHandler, PingHandler>();

        // Validators
        services.AddScoped<IValidator<PingRequest>, PingRequestValidator>();

        return services;
    }
}
