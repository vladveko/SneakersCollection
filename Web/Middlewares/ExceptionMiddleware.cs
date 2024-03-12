using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var problemDetails = new ProblemDetails()
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Internal server error occurred"
        };

        var response = context.Response;
        response.ContentType = "application/json";
        response.StatusCode = problemDetails.Status!.Value;
        await response.WriteAsync(JsonSerializer.Serialize(problemDetails));
    }
}