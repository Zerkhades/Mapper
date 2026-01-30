using AutoMapper;
using Mapper.Application.Common.Mappings;
using Mapper.Application.Features.DTOs;
using Mapper.Domain;

namespace Mapper.Tests.Mappings;

public class CameraArchiveMappingTests
{
    private readonly IMapper _mapper;

    public CameraArchiveMappingTests()
    {
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CameraArchiveProfile>();
        });

        _mapper = configuration.CreateMapper();
    }

    [Fact]
    public void CameraVideoArchive_ShouldMapToDto()
    {
        // Arrange
        var cameraMarkId = Guid.NewGuid();
        var archive = new CameraVideoArchive(
            cameraMarkId,
            "/videos/test.mp4",
            TimeSpan.FromMinutes(5),
            10485760,
            "1920x1080",
            30,
            "/thumbnails/test.jpg"
        );

        // Act
        var dto = _mapper.Map<CameraVideoArchiveDto>(archive);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(archive.Id, dto.Id);
        Assert.Equal(cameraMarkId, dto.CameraMarkId);
        Assert.Equal("/videos/test.mp4", dto.VideoPath);
        Assert.Equal("/thumbnails/test.jpg", dto.ThumbnailPath);
        Assert.Equal(TimeSpan.FromMinutes(5), dto.Duration);
        Assert.Equal(10485760, dto.FileSizeBytes);
        Assert.Equal("1920x1080", dto.Resolution);
        Assert.Equal(30, dto.FramesPerSecond);
    }

    [Fact]
    public void CameraMotionAlert_ShouldMapToDto()
    {
        // Arrange
        var cameraMarkId = Guid.NewGuid();
        var alert = new CameraMotionAlert(
            cameraMarkId,
            MotionSeverity.High,
            85.5,
            "/snapshots/alert.jpg"
        );

        // Act
        var dto = _mapper.Map<CameraMotionAlertDto>(alert);

        // Assert
        Assert.NotNull(dto);
        Assert.Equal(alert.Id, dto.Id);
        Assert.Equal(cameraMarkId, dto.CameraMarkId);
        Assert.Equal("High", dto.Severity);
        Assert.Equal(85.5, dto.MotionPercentage);
        // SnapshotPath in domain maps to SnapshotUrl in DTO
        Assert.NotNull(dto.SnapshotUrl);
    }

    [Theory]
    [InlineData(MotionSeverity.Low, "Low")]
    [InlineData(MotionSeverity.Medium, "Medium")]
    [InlineData(MotionSeverity.High, "High")]
    public void CameraMotionAlert_ShouldMapSeverityCorrectly(MotionSeverity severity, string expectedString)
    {
        // Arrange
        var alert = new CameraMotionAlert(Guid.NewGuid(), severity, 50.0);

        // Act
        var dto = _mapper.Map<CameraMotionAlertDto>(alert);

        // Assert
        Assert.Equal(expectedString, dto.Severity);
    }

    [Fact]
    public void AutoMapperConfiguration_ShouldBeValid()
    {
        // Arrange & Act & Assert
        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CameraArchiveProfile>();
        });

        configuration.AssertConfigurationIsValid();
    }
}
