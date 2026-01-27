using Application.Interfaces;
using Infrastructure.Helpers;
using Infrastructure.JwtAuth;
using Infrastructure.Redis;
using Infrastructure.Repositories;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // IOptions<T>
        services.Configure<RedisSettings>(config.GetSection("Redis"));
        services.Configure<JwtSettings>(config.GetSection("Jwt"));

        // Binds Jwt settings from app-settings.json to JwtSettings
        services.AddHttpContextAccessor();

        // Configure JWT authentication and authorization
        services.AddJwtAuthentication(config);

        /*services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisSettings = sp
                .GetRequiredService<IOptions<RedisSettings>>()
                .Value;

            return ConnectionMultiplexer.Connect(
                redisSettings.ConnectionString + ",abortConnect=false"
            );
        });*/
        
        // Configure Redis connection for caching and token blacklist
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(
                config["Redis:ConnectionString"] + ",abortConnect=false" // prevents the app from crashing if Redis is not available at startup
            )
        );
        
        // Register repositories and unit of work for database access
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Register application services (JWT, blacklist, helpers)
        services.AddScoped<IJwtService, JwtTokenService>();
        services.AddScoped<IJwtBlacklistService, JwtBlacklistService>();

        return services;
    }
}
