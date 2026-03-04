using VerticalSliceBoilerplate.Shared.Api.Endpoints;

namespace VerticalSliceBoilerplate.Api.Features.Sample;

public static class SampleEndpoints
{
    public static IServiceCollection AddSampleEndpoints(this IServiceCollection services)
    {
        services.AddEndpointsFromNamespace(
            typeof(SampleEndpoints).Assembly,
            typeof(SampleEndpoints).Namespace!);
        return services;
    }
}
