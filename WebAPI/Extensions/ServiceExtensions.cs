using Application;
using Infrastructure;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace JWTAuth.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Application layer
        services.AddApplication(); 
        services.AddInfrastructure(configuration);
        
        /*// Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DB")));*/
        
        // ✅ Database: SQL Server connection resiliency
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DB"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,                   
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);        

                    sql.CommandTimeout(30); 
                }));
        
        services.AddHealthChecks()
            .AddDbContextCheck<AppDbContext>(
                name: "db",
                failureStatus: HealthStatus.Unhealthy)
            .AddRedis(
                configuration["Redis:ConnectionString"]!,
                name: "redis",
                failureStatus: HealthStatus.Unhealthy);


        services.AddControllers();
        return services;
    }
}