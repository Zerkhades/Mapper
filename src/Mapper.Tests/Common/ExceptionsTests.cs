using Mapper.Application.Common.Exceptions;

namespace Mapper.Tests.Common;

public class ExceptionsTests
{
    [Fact]
    public void NotFoundException_WithNameAndKey_ShouldSetMessage()
    {
        // Arrange
        var name = "Employee";
        var key = Guid.NewGuid();

        // Act
        var exception = new NotFoundException(name, key);

        // Assert
        Assert.Contains(name, exception.Message);
        Assert.Contains(key.ToString(), exception.Message);
        Assert.Contains("not found", exception.Message);
    }

    [Fact]
    public void NotFoundException_ShouldFormatMessageCorrectly()
    {
        // Arrange
        var entityName = "GeoMap";
        var entityId = Guid.NewGuid();

        // Act
        var exception = new NotFoundException(entityName, entityId);

        // Assert
        var expectedMessage = $"Entity \"{entityName}\" ({entityId}) not found.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AlreadyExistsException_WithNameAndKey_ShouldSetMessage()
    {
        // Arrange
        var name = "Employee";
        var key = "john.doe@example.com";

        // Act
        var exception = new AlreadyExistsException(name, key);

        // Assert
        Assert.Contains(name, exception.Message);
        Assert.Contains(key, exception.Message);
        Assert.Contains("already exists", exception.Message);
    }

    [Fact]
    public void AlreadyExistsException_ShouldFormatMessageCorrectly()
    {
        // Arrange
        var name = "User";
        var key = "admin@test.com";

        // Act
        var exception = new AlreadyExistsException(name, key);

        // Assert
        var expectedMessage = $"Entity \"{name}\" ({key}) is already exists in DB.";
        Assert.Equal(expectedMessage, exception.Message);
    }

    [Fact]
    public void AlreadyExistsException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new AlreadyExistsException("Test", "key");

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }

    [Fact]
    public void NotFoundException_ShouldInheritFromException()
    {
        // Arrange & Act
        var exception = new NotFoundException("Test", Guid.NewGuid());

        // Assert
        Assert.IsAssignableFrom<Exception>(exception);
    }
}
