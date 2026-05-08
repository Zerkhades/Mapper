using Mapper.Application.Features.Analytics;

namespace Mapper.Tests.Analytics;

public class MotionAnomalyDetectorTests
{
    [Fact]
    public void Analyze_WithActivitySpike_ShouldFlagAnomaly()
    {
        // Arrange
        var cameraId = Guid.NewGuid();
        var input = new MotionAnomalyInput(cameraId, "Entrance", 8, new[] { 1 });

        // Act
        var result = MotionAnomalyDetector.Analyze(input);

        // Assert
        Assert.True(result.IsAnomaly);
        Assert.Equal(cameraId, result.CameraMarkId);
        Assert.Equal(8, result.CurrentAlertCount);
        Assert.True(result.ActivityRatio >= 2.5);
    }

    [Fact]
    public void Analyze_WithExpectedActivity_ShouldNotFlagAnomaly()
    {
        // Arrange
        var input = new MotionAnomalyInput(Guid.NewGuid(), "Entrance", 2, new[] { 2 });

        // Act
        var result = MotionAnomalyDetector.Analyze(input);

        // Assert
        Assert.False(result.IsAnomaly);
    }
}
