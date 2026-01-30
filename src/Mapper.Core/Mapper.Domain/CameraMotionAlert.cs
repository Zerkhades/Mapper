namespace Mapper.Domain;

public enum MotionSeverity
{
    Low = 1,
    Medium = 2,
    High = 3
}

public class CameraMotionAlert
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid CameraMarkId { get; private set; }
    public DateTimeOffset DetectedAt { get; private set; }
    public DateTimeOffset? ConfirmedAt { get; private set; }
    public MotionSeverity Severity { get; private set; }
    public double MotionPercentage { get; private set; } // 0-100, percentage of frame with motion
    public string? SnapshotPath { get; private set; }
    public bool IsResolved { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public Guid? RelatedVideoArchiveId { get; private set; }

    private CameraMotionAlert() { } // EF

    public CameraMotionAlert(
        Guid cameraMarkId,
        MotionSeverity severity,
        double motionPercentage,
        string? snapshotPath = null)
    {
        CameraMarkId = cameraMarkId;
        DetectedAt = DateTimeOffset.UtcNow;
        Severity = severity;
        MotionPercentage = motionPercentage;
        SnapshotPath = snapshotPath;
    }

    public void Confirm() => ConfirmedAt = DateTimeOffset.UtcNow;

    public void Resolve(string? notes = null)
    {
        IsResolved = true;
        ResolutionNotes = notes;
    }

    public void LinkToVideo(Guid videoArchiveId) => RelatedVideoArchiveId = videoArchiveId;
}
