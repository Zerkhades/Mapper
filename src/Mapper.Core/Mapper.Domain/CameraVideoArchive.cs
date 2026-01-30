namespace Mapper.Domain;

public class CameraVideoArchive
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid CameraMarkId { get; private set; }
    public string VideoPath { get; private set; } = default!;
    public string? ThumbnailPath { get; private set; }
    public DateTimeOffset RecordedAt { get; private set; }
    public TimeSpan Duration { get; private set; }
    public long FileSizeBytes { get; private set; }
    public bool HasMotionDetected { get; private set; }
    public string Resolution { get; private set; } = default!; // e.g., "1920x1080"
    public int FramesPerSecond { get; private set; }
    public bool IsArchived { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private CameraVideoArchive() { } // EF

    public CameraVideoArchive(
        Guid cameraMarkId,
        string videoPath,
        TimeSpan duration,
        long fileSizeBytes,
        string resolution,
        int fps,
        string? thumbnailPath = null)
    {
        CameraMarkId = cameraMarkId;
        VideoPath = videoPath;
        Duration = duration;
        FileSizeBytes = fileSizeBytes;
        Resolution = resolution;
        FramesPerSecond = fps;
        ThumbnailPath = thumbnailPath;
        RecordedAt = DateTimeOffset.UtcNow;
    }

    public void SetMotionDetected(bool detected) => HasMotionDetected = detected;

    public void Archive() => IsArchived = true;
}
