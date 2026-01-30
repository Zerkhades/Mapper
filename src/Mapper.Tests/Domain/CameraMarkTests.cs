using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class CameraMarkTests
{
    [Fact]
    public void Constructor_ShouldCreateCameraMarkWithValidProperties()
    {
        // Arrange
        var geoMapId = Guid.NewGuid();
        var x = 100.5;
        var y = 200.7;
        var title = "Camera 1";
        var cameraName = "Front Entrance";
        var streamUrl = "rtsp://192.168.1.10/stream";
        var description = "Main entrance camera";

        // Act
        var cameraMark = new CameraMark(geoMapId, x, y, title, cameraName, streamUrl, description);

        // Assert
        Assert.NotEqual(Guid.Empty, cameraMark.Id);
        Assert.Equal(geoMapId, cameraMark.GeoMapId);
        Assert.Equal(x, cameraMark.X);
        Assert.Equal(y, cameraMark.Y);
        Assert.Equal(title, cameraMark.Title);
        Assert.Equal(cameraName, cameraMark.CameraName);
        Assert.Equal(streamUrl, cameraMark.StreamUrl);
        Assert.Equal(description, cameraMark.Description);
        Assert.Equal(GeoMarkType.Camera, cameraMark.Type);
        Assert.False(cameraMark.IsDeleted);
    }

    [Fact]
    public void UpdateCamera_ShouldModifyCameraProperties()
    {
        // Arrange
        var cameraMark = new CameraMark(Guid.NewGuid(), 10, 20, "Camera", "Old Name", "old-url");
        var newName = "New Camera Name";
        var newUrl = "rtsp://new-url";

        // Act
        cameraMark.UpdateCamera(newName, newUrl);

        // Assert
        Assert.Equal(newName, cameraMark.CameraName);
        Assert.Equal(newUrl, cameraMark.StreamUrl);
    }

    [Fact]
    public void Move_ShouldUpdateCoordinates()
    {
        // Arrange
        var cameraMark = new CameraMark(Guid.NewGuid(), 10, 20, "Camera", "Name", "url");
        var newX = 30.5;
        var newY = 40.7;

        // Act
        cameraMark.Move(newX, newY);

        // Assert
        Assert.Equal(newX, cameraMark.X);
        Assert.Equal(newY, cameraMark.Y);
    }

    [Fact]
    public void UpdateText_ShouldModifyTitleAndDescription()
    {
        // Arrange
        var cameraMark = new CameraMark(Guid.NewGuid(), 10, 20, "Old Title", "Name", "url");
        var newTitle = "New Title";
        var newDescription = "New Description";

        // Act
        cameraMark.UpdateText(newTitle, newDescription);

        // Assert
        Assert.Equal(newTitle, cameraMark.Title);
        Assert.Equal(newDescription, cameraMark.Description);
    }

    [Fact]
    public void SoftDelete_ShouldMarkAsDeleted()
    {
        // Arrange
        var cameraMark = new CameraMark(Guid.NewGuid(), 10, 20, "Camera", "Name", "url");

        // Act
        cameraMark.SoftDelete();

        // Assert
        Assert.True(cameraMark.IsDeleted);
        Assert.NotNull(cameraMark.DeletedAt);
    }
}
