using System.Net;
using System.Text.Json;

namespace BibliotecaApp.API.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await ManejarError(context, ex);
        }
    }

    private async Task ManejarError(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var statusCode = ex switch
        {
            ArgumentException => HttpStatusCode.BadRequest,
            KeyNotFoundException => HttpStatusCode.NotFound,
            UnauthorizedAccessException => HttpStatusCode.Unauthorized,
            _ => HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = (int)statusCode;

        var respuesta = new
        {
            status = (int)statusCode,
            mensaje = ex.Message,
            stackTrace = ex.StackTrace,
            fecha = DateTime.UtcNow
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(respuesta));
    }
}