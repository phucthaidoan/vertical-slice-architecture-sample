using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace VerticalSliceBoilerplate.Infrastructure.Data.Postgres;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPostgresInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        string connectionStringName = "Default")
    {
        var connectionString = configuration.GetConnectionString(connectionStringName)
                              ?? throw new InvalidOperationException($"Connection string '{connectionStringName}' not found.");

        services.AddScoped<DomainEventInterceptor>();

        services.AddDbContext<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<DomainEventInterceptor>();

            options.UseNpgsql(connectionString)
                .AddInterceptors(interceptor);
        });

        return services;
    }
}

