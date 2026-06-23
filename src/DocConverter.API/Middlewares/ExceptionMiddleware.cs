using System.Net;
using System.Text.Json;
using DocConverter.Domain.Exceptions;

namespace DocConverter.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Deja pasar la petición al siguiente componente (Controlador)
            await _next(context);
        }
        catch (Exception ex)
        {
            // Si algo falla en cualquier parte del sistema, cae acá
            _logger.LogError(ex, "Ocurrió una excepción no controlada: {Message}", ex.Message);
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        // Mapeo la excepción de negocio al código de estado HTTP correcto
        var statusCode = exception switch
        {
            BadRequestException => HttpStatusCode.BadRequest,      // 400
            UnauthorizedException => HttpStatusCode.Unauthorized,  // 401
            NotFoundException => HttpStatusCode.NotFound,          // 404
            KeyNotFoundException => HttpStatusCode.NotFound,       // 404
            _ => HttpStatusCode.InternalServerError                // 500 (Errores inesperados)
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            StatusCode = context.Response.StatusCode,
            Message = exception.Message,
            Details = _env.IsDevelopment() ? exception.StackTrace?.ToString() : null
        };

        var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var jsonResult = JsonSerializer.Serialize(response, jsonOptions);

        return context.Response.WriteAsync(jsonResult);
    }
}