using System.Net;
using System.Text.Json;
using ExitCafe.Application.Common.Models;
using ExitCafe.Domain.Exceptions;
using FluentValidation;
using Serilog;

namespace ExitCafe.WebApi.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationException => (
                HttpStatusCode.BadRequest,
                "One or more validation errors occurred.",
                validationException.Errors.Select(e => e.ErrorMessage).ToList()),
            NotFoundException => (HttpStatusCode.NotFound, exception.Message, (List<string>?)null),
            BadRequestException => (HttpStatusCode.BadRequest, exception.Message, (List<string>?)null),
            ConflictException => (HttpStatusCode.Conflict, exception.Message, (List<string>?)null),
            UnauthorizedException => (HttpStatusCode.Unauthorized, exception.Message, (List<string>?)null),
            ForbiddenException => (HttpStatusCode.Forbidden, exception.Message, (List<string>?)null),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.", (List<string>?)null)
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            Log.Error(exception, "Unhandled exception occurred");
        else
            Log.Warning("Handled exception: {Message}", exception.Message);

        context.Response.StatusCode = (int)statusCode;

        var response = ApiResponse<object>.Fail(message, errors);
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}
