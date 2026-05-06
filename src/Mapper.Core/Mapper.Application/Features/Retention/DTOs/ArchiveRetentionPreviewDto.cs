namespace Mapper.Application.Features.Retention.DTOs;

public class ArchiveRetentionPreviewDto
{
    public DateTimeOffset GeneratedAt { get; set; }
    public int MotionVideoRetentionDays { get; set; }
    public int NoMotionVideoRetentionDays { get; set; }
    public int ArchivedVideoRetentionDays { get; set; }
    public int CandidateCount { get; set; }
    public long ReclaimableBytes { get; set; }
    public List<ArchiveRetentionCandidateDto> Candidates { get; set; } = [];
}

public class ArchiveRetentionCandidateDto
{
    public Guid VideoArchiveId { get; set; }
    public Guid CameraMarkId { get; set; }
    public string VideoPath { get; set; } = default!;
    public string? ThumbnailPath { get; set; }
    public DateTimeOffset RecordedAt { get; set; }
    public long FileSizeBytes { get; set; }
    public bool HasMotionDetected { get; set; }
    public bool IsArchived { get; set; }
    public int AgeDays { get; set; }
    public int RetentionDays { get; set; }
    public string Reason { get; set; } = default!;
}
