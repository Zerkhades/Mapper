using Amazon.S3;
using FluentValidation;
using Mapper.Application.Common.Exceptions;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace Mapper.WebApi.Middleware
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(
            RequestDelegate next,
            IWebHostEnvironment environment,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _environment = environment;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled request exception");
                await HandleExceptionAsync(context, exception);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            ProblemDetails problemDetails;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    problemDetails = new ValidationProblemDetails(
                        validationException.Errors
                            .GroupBy(error => error.PropertyName)
                            .ToDictionary(
                                group => group.Key,
                                group => group.Select(error => error.ErrorMessage).ToArray()))
                    {
                        Title = "Validation failed.",
                        Status = (int)code,
                        Type = "https://tools.ietf.org/html/rfc9110#section-15.5.1",
                        Instance = context.Request.Path
                    };
                    break;
                case NotFoundException:
                    code = HttpStatusCode.NotFound;
                    problemDetails = CreateProblemDetails(context, code, "Resource was not found.", exception.Message);
                    break;
                case AmazonS3Exception s3Ex:
                    code = HttpStatusCode.BadRequest;
                    problemDetails = CreateProblemDetails(context, code, "S3 request failed.", s3Ex.Message);
                    problemDetails.Extensions["service"] = "S3";
                    break;
                default:
                    problemDetails = CreateProblemDetails(
                        context,
                        code,
                        "An unexpected error occurred.",
                        _environment.IsDevelopment() ? exception.Message : null);
                    break;
            }

            context.Response.ContentType = "application/problem+json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, problemDetails.GetType()));
        }

        private static ProblemDetails CreateProblemDetails(
            HttpContext context,
            HttpStatusCode statusCode,
            string title,
            string? detail)
        {
            return new ProblemDetails
            {
                Title = title,
                Detail = detail,
                Status = (int)statusCode,
                Type = $"https://tools.ietf.org/html/rfc9110#section-{GetRfcSection(statusCode)}",
                Instance = context.Request.Path
            };
        }

        private static string GetRfcSection(HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                HttpStatusCode.BadRequest => "15.5.1",
                HttpStatusCode.NotFound => "15.5.5",
                _ => "15.6.1"
            };
        }
    }
}
