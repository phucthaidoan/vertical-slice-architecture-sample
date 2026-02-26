using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceBoilerplate.Core.Features.Sample.Ping;
using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Core.Features.Sample;

public static class SampleFeature
{
    public static IServiceCollection AddSampleFeature(this IServiceCollection services)
    {
        services.AddEndpointsFromNamespace(typeof(SampleFeature).Assembly, typeof(SampleFeature).Namespace!);

        services.AddScoped<IPingHandler, PingHandler>();

        // Register validators explicitly to avoid extra package dependencies.
        services.AddScoped<IValidator<PingRequest>, PingRequestValidator>();

        return services;
    }
}

