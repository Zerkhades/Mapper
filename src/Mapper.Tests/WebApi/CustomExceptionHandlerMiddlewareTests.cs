using FluentValidation;
using FluentValidation.Results;
using Mapper.Application.Common.Exceptions;
using Mapper.WebApi.Middleware;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Mapper.Tests.WebApi;

public class CustomExceptionHandlerMiddlewareTests
{
    [Fact]
    public async Task Invoke_WithValidationException_ShouldReturnValidationProblemDetails()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = CreateMiddleware(_ => throw new ValidationException(new[]
        {
            new ValidationFailure("Name", "Name is required")
        }));

        // Act
        await middleware.Invoke(context);

        // Assert
        var body = await ReadBody(context);
        Assert.Equal(StatusCodes.Status400BadRequest, context.Response.StatusCode);
        Assert.Equal("application/problem+json", context.Response.ContentType);
        Assert.Contains("Validation failed.", body);
        Assert.Contains("Name is required", body);
    }

    [Fact]
    public async Task Invoke_WithNotFoundException_ShouldReturnProblemDetails()
    {
        // Arrange
        var id = Guid.NewGuid();
        var context = CreateHttpContext();
        var middleware = CreateMiddleware(_ => throw new NotFoundException("GeoMap", id));

        // Act
        await middleware.Invoke(context);

        // Assert
        var body = await ReadBody(context);
        using var document = JsonDocument.Parse(body);
        Assert.Equal(StatusCodes.Status404NotFound, context.Response.StatusCode);
        Assert.Equal("Resource was not found.", document.RootElement.GetProperty("title").GetString());
        Assert.Contains(id.ToString(), document.RootElement.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task Invoke_WithUnexpectedExceptionOutsideDevelopment_ShouldHideExceptionMessage()
    {
        // Arrange
        var context = CreateHttpContext();
        var middleware = CreateMiddleware(_ => throw new InvalidOperationException("Sensitive details"), "Production");

        // Act
        await middleware.Invoke(context);

        // Assert
        var body = await ReadBody(context);
        Assert.Equal(StatusCodes.Status500InternalServerError, context.Response.StatusCode);
        Assert.DoesNotContain("Sensitive details", body);
        Assert.Contains("An unexpected error occurred.", body);
    }

    private static CustomExceptionHandlerMiddleware CreateMiddleware(
        RequestDelegate next,
        string environmentName = "Development")
    {
        return new CustomExceptionHandlerMiddleware(
            next,
            new TestWebHostEnvironment(environmentName),
            NullLogger<CustomExceptionHandlerMiddleware>.Instance);
    }

    private static DefaultHttpContext CreateHttpContext()
    {
        return new DefaultHttpContext
        {
            Response =
            {
                Body = new MemoryStream()
            }
        };
    }

    private static async Task<string> ReadBody(HttpContext context)
    {
        context.Response.Body.Position = 0;
        using var reader = new StreamReader(context.Response.Body);
        return await reader.ReadToEndAsync();
    }

    private sealed class TestWebHostEnvironment : IWebHostEnvironment
    {
        public TestWebHostEnvironment(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; } = "Mapper.Tests";
        public string WebRootPath { get; set; } = string.Empty;
        public IFileProvider WebRootFileProvider { get; set; } = new NullFileProvider();
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
