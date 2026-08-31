namespace Cochief.Infrastructure.Middleware;

using Cochief.Application.Exceptions;
using Cochief.Domain.Exceptions;

public sealed class ExceptionHandlerMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception) when (
            !context.Response.HasStarted &&
            !context.RequestAborted.IsCancellationRequested)
        {
            await HandleExceptionAsync(context, exception);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        (int statusCode, string title, string detail) = MapException(exception);

        context.Response.Clear();

        await Results.Problem(
            statusCode: statusCode,
            title: title,
            detail: detail,
            instance: context.Request.Path,
            extensions: new Dictionary<string, object?>
            {
                ["traceId"] = context.TraceIdentifier
            }).ExecuteAsync(context);
    }

    private static (int StatusCode, string Title, string Detail) MapException(Exception exception) =>
        exception switch
    {
        AuthException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
        ValidationException => (StatusCodes.Status400BadRequest, "Bad Request", exception.Message),
        DomainException => (StatusCodes.Status409Conflict, "Conflict", exception.Message),
        _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", "An unexpected error occurred.")
    };
}
