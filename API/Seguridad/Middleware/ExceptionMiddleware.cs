using System.Text.Json;
using API.Seguridad.Application.Core;
using API.Seguridad.Infrastructure;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Serilog.Context;

namespace API.Seguridad.Middleware
{
    public class ExceptionMiddleware(ILogger<ExceptionMiddleware> logger, IHostEnvironment env)
        : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ValidationException ex)
            {
                await HandleValidationException(context, ex);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception ex)
        {
            var userId = AuthContext.GetUserId(context.User) ?? "System";
            var userName = AuthContext.GetUserName(context.User) ?? "System";

            using (LogContext.PushProperty("UserId", userId))
            using (LogContext.PushProperty("UserName", userName))
            {
                logger.LogError(ex, ex.Message);
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = env.IsDevelopment()
                ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace)
                : new AppException(context.Response.StatusCode, ex.Message, null);

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

            var json = JsonSerializer.Serialize(response, options);

            await context.Response.WriteAsync(json);
        }

        private static async Task HandleValidationException(HttpContext context, ValidationException ex)
        {
            var validationErrors = new Dictionary<string, string[]>();

            if (ex.Errors is not null)
            {
                foreach (var error in ex.Errors)
                {
                    if (validationErrors.TryGetValue(error.PropertyName, out var existingErrors))
                    {
                        validationErrors[error.PropertyName] = [.. existingErrors, error.ErrorMessage];
                    }
                    else
                    {
                        validationErrors[error.PropertyName] = [error.ErrorMessage];
                    }
                }
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var validationProblemDetails = new ValidationProblemDetails(validationErrors)
            {
                Status = StatusCodes.Status400BadRequest,
                Type = "ValidationFailure",
                Title = "Error en validación",
                Detail = "Uno o más errores de validación han ocurrido"
            };

            await context.Response.WriteAsJsonAsync(validationProblemDetails);
        }
    }
}