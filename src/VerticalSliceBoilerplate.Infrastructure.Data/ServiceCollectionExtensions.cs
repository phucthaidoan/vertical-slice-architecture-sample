using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VerticalSliceBoilerplate.Infrastructure.Data.Context;
using VerticalSliceBoilerplate.Infrastructure.Data.DomainEvents;
using VerticalSliceBoilerplate.Infrastructure.Data.Interceptors;

namespace VerticalSliceBoilerplate.Infrastructure.Data;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDataInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "Default")
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
                              ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        // Register domain event dispatcher
        services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

        // Register interceptors
        services.AddScoped<DomainEventInterceptor>();

        // Register DbContext with interceptors
        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventInterceptor>();

            options.UseNpgsql(connectionString)
                .AddInterceptors(interceptor);
        });

        return services;
    }
}

