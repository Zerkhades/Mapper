using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class GeoMapTests
{
    [Fact]
    public void Constructor_ShouldCreateGeoMapWithValidProperties()
    {
        // Arrange
        var name = "Test Map";
        var imagePath = "/maps/test.png";
        var width = 1920;
        var height = 1080;
        var description = "Test description";

        // Act
        var map = new GeoMap(name, imagePath, width, height, description);

        // Assert
        Assert.NotEqual(Guid.Empty, map.Id);
        Assert.Equal(name, map.Name);
        Assert.Equal(imagePath, map.ImagePath);
        Assert.Equal(width, map.ImageWidth);
        Assert.Equal(height, map.ImageHeight);
        Assert.Equal(description, map.Description);
        Assert.False(map.IsDeleted);
        Assert.Null(map.DeletedAt);
    }

    [Fact]
    public void Update_ShouldModifyNameAndDescription()
    {
        // Arrange
        var map = new GeoMap("Original", "/path.png", 100, 100);
        var newName = "Updated Name";
        var newDescription = "Updated Description";

        // Act
        map.Update(newName, newDescription);

        // Assert
        Assert.Equal(newName, map.Name);
        Assert.Equal(newDescription, map.Description);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var map = new GeoMap("Test", "/path.png", 100, 100);
        var deletedAt = DateTimeOffset.UtcNow;

        // Act
        map.SoftDelete(deletedAt);

        // Assert
        Assert.True(map.IsDeleted);
        Assert.Equal(deletedAt, map.DeletedAt);
    }

    [Fact]
    public void SoftDelete_WithoutDate_ShouldUseCurrentTime()
    {
        // Arrange
        var map = new GeoMap("Test", "/path.png", 100, 100);
        var beforeDelete = DateTimeOffset.UtcNow;

        // Act
        map.SoftDelete();

        // Assert
        Assert.True(map.IsDeleted);
        Assert.NotNull(map.DeletedAt);
        Assert.True(map.DeletedAt >= beforeDelete);
    }

    [Fact]
    public void SoftDelete_CalledTwice_ShouldNotChangeDeletedAt()
    {
        // Arrange
        var map = new GeoMap("Test", "/path.png", 100, 100);
        var firstDeleteTime = DateTimeOffset.UtcNow.AddMinutes(-10);
        map.SoftDelete(firstDeleteTime);

        // Act
        map.SoftDelete(DateTimeOffset.UtcNow);

        // Assert
        Assert.Equal(firstDeleteTime, map.DeletedAt);
    }
}
