using Mapper.Application.Features.GeoMaps.Commands.CreateGeoMap;

namespace Mapper.Tests.Validators;

public class CreateGeoMapValidatorTests
{
    private readonly CreateGeoMapValidator _validator;

    public CreateGeoMapValidatorTests()
    {
        _validator = new CreateGeoMapValidator();
    }

    [Fact]
    public void Validator_ValidCommand_ShouldNotHaveErrors()
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: "Test Map",
            Description: "Test description",
            ImageStream: stream,
            FileName: "map.png",
            ContentType: "image/png",
            ImageWidth: 1920,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Validator_EmptyName_ShouldHaveError(string? name)
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: name!,
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: "image/png",
            ImageWidth: 1920,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Fact]
    public void Validator_TooLongName_ShouldHaveError()
    {
        // Arrange
        var longName = new string('A', 201);
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: longName,
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: "image/png",
            ImageWidth: 1920,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.Name));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validator_InvalidImageWidth_ShouldHaveError(int width)
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: "Test Map",
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: "image/png",
            ImageWidth: width,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ImageWidth));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validator_InvalidImageHeight_ShouldHaveError(int height)
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: "Test Map",
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: "image/png",
            ImageWidth: 1920,
            ImageHeight: height
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ImageHeight));
    }

    [Theory]
    [InlineData("image/png")]
    [InlineData("image/jpeg")]
    public void Validator_ValidContentType_ShouldNotHaveError(string contentType)
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: "Test Map",
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: contentType,
            ImageWidth: 1920,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("image/gif")]
    [InlineData("image/bmp")]
    [InlineData("application/pdf")]
    [InlineData("text/plain")]
    public void Validator_InvalidContentType_ShouldHaveError(string contentType)
    {
        // Arrange
        using var stream = new MemoryStream();
        var command = new CreateGeoMapCommand(
            Name: "Test Map",
            Description: null,
            ImageStream: stream,
            FileName: "map.png",
            ContentType: contentType,
            ImageWidth: 1920,
            ImageHeight: 1080
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(command.ContentType));
    }
}
