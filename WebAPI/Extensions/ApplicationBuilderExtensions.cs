using JWTAuth.Middlewares;

namespace JWTAuth.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseCustomMiddlewares(this IApplicationBuilder app)
    {
        app.UseCustomExceptionHandler();
        app.UseHttpsRedirection(); // HTTP -> HTTPS

        app.UseAuthentication();
        app.UseMiddleware<JwtBlacklistMiddleware>();
        app.UseAuthorization();
        return app;
    }
}
