using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class TransitionMarkTests
{
    [Fact]
    public void Constructor_ShouldCreateTransitionMarkWithValidProperties()
    {
        // Arrange
        var geoMapId = Guid.NewGuid();
        var targetGeoMapId = Guid.NewGuid();
        var x = 300.0;
        var y = 400.0;
        var title = "Door to Floor 2";
        var description = "Main staircase";

        // Act
        var transitionMark = new TransitionMark(geoMapId, x, y, title, targetGeoMapId, description);

        // Assert
        Assert.NotEqual(Guid.Empty, transitionMark.Id);
        Assert.Equal(geoMapId, transitionMark.GeoMapId);
        Assert.Equal(targetGeoMapId, transitionMark.TargetGeoMapId);
        Assert.Equal(x, transitionMark.X);
        Assert.Equal(y, transitionMark.Y);
        Assert.Equal(title, transitionMark.Title);
        Assert.Equal(description, transitionMark.Description);
        Assert.Equal(GeoMarkType.Transition, transitionMark.Type);
    }

    [Fact]
    public void SetTarget_ShouldUpdateTargetGeoMapId()
    {
        // Arrange
        var oldTarget = Guid.NewGuid();
        var transitionMark = new TransitionMark(Guid.NewGuid(), 10, 20, "Door", oldTarget);
        var newTarget = Guid.NewGuid();

        // Act
        transitionMark.SetTarget(newTarget);

        // Assert
        Assert.Equal(newTarget, transitionMark.TargetGeoMapId);
    }
}
