using System.ComponentModel.DataAnnotations;
using Application.Exceptions;

namespace JWTAuth.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (errorResponse, statusCode) = CreateErrorResponse(exception);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;
        await context.Response.WriteAsJsonAsync(new
        {
            errorResponse,
            StatusCode = statusCode
        });
    }

    private static (object ErrorResponse, int StatusCode) CreateErrorResponse(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => (
                new
                {
                    Message = "Validation error",
                    Errors = validationException
                },
                StatusCodes.Status400BadRequest),
            // ✅ MANA SHU JOYGA QO‘SHASAN
            ServiceUnavailableException => (
                new
                {
                    exception.Message
                },
                StatusCodes.Status503ServiceUnavailable),
            NotFoundException => (
                new
                {
                    exception.Message
                },
                StatusCodes.Status404NotFound),

            ApiException => (
                new
                {
                    exception.Message
                },
                StatusCodes.Status400BadRequest),

            _ => (
                new
                {
                    Message = "Internal server error",
                    Details = exception.Message
                },
                StatusCodes.Status500InternalServerError)
        };
    }
}
