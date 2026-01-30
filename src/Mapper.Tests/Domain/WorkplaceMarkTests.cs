using Mapper.Domain;

namespace Mapper.Tests.Domain;

public class WorkplaceMarkTests
{
    [Fact]
    public void Constructor_ShouldCreateWorkplaceMarkWithValidProperties()
    {
        // Arrange
        var geoMapId = Guid.NewGuid();
        var x = 150.0;
        var y = 250.0;
        var title = "Office 101";
        var workplaceCode = "WP-101";
        var description = "Marketing department";

        // Act
        var workplaceMark = new WorkplaceMark(geoMapId, x, y, title, workplaceCode, description);

        // Assert
        Assert.NotEqual(Guid.Empty, workplaceMark.Id);
        Assert.Equal(geoMapId, workplaceMark.GeoMapId);
        Assert.Equal(x, workplaceMark.X);
        Assert.Equal(y, workplaceMark.Y);
        Assert.Equal(title, workplaceMark.Title);
        Assert.Equal(workplaceCode, workplaceMark.WorkplaceCode);
        Assert.Equal(description, workplaceMark.Description);
        Assert.Equal(GeoMarkType.Workplace, workplaceMark.Type);
    }

    [Fact]
    public void SetWorkplaceCode_ShouldUpdateCode()
    {
        // Arrange
        var workplaceMark = new WorkplaceMark(Guid.NewGuid(), 10, 20, "Office", "OLD-CODE");
        var newCode = "NEW-CODE-123";

        // Act
        workplaceMark.SetWorkplaceCode(newCode);

        // Assert
        Assert.Equal(newCode, workplaceMark.WorkplaceCode);
    }
}
