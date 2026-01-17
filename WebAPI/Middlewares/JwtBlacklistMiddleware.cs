using System.IdentityModel.Tokens.Jwt;
using Application.Exceptions;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace JWTAuth.Middlewares;

public class JwtBlacklistMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context, IJwtBlacklistService blacklistService)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
        {
            await _next(context);
            return;
        }

        
        var authHeader = context.Request.Headers.Authorization.FirstOrDefault();
        var token = authHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
            ? authHeader["Bearer ".Length..].Trim()
            : null;

        if (!string.IsNullOrWhiteSpace(token))
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();

                var jwt = handler.ReadJwtToken(token);
                var jti = jwt.Id;

                if (!string.IsNullOrWhiteSpace(jti))
                {
                    var revoked = await blacklistService.ExistsAsync(jti);
                    if (revoked)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        await context.Response.WriteAsync("Token revoked");
                        return;
                    }
                }
            }
            catch
            {
                throw new ApiException("Invalid in token");
                
            }
        }

        await _next(context);
    }
}
