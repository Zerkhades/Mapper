using Mapper.Application.Features.Retention.DTOs;

namespace Mapper.Application.Features.Retention;

public static class ArchiveRetentionPolicy
{
    public static ArchiveRetentionCandidateDto? BuildCandidate(
        Guid id,
        Guid cameraMarkId,
        string videoPath,
        string? thumbnailPath,
        DateTimeOffset recordedAt,
        long fileSizeBytes,
        bool hasMotionDetected,
        bool isArchived,
        DateTimeOffset now,
        int motionRetentionDays,
        int noMotionRetentionDays,
        int archivedRetentionDays)
    {
        var ageDays = Math.Max(0, (int)Math.Floor((now - recordedAt).TotalDays));
        var retentionDays = ResolveRetentionDays(hasMotionDetected, isArchived, motionRetentionDays, noMotionRetentionDays, archivedRetentionDays);
        if (ageDays < retentionDays)
        {
            return null;
        }

        return new ArchiveRetentionCandidateDto
        {
            VideoArchiveId = id,
            CameraMarkId = cameraMarkId,
            VideoPath = videoPath,
            ThumbnailPath = thumbnailPath,
            RecordedAt = recordedAt,
            FileSizeBytes = fileSizeBytes,
            HasMotionDetected = hasMotionDetected,
            IsArchived = isArchived,
            AgeDays = ageDays,
            RetentionDays = retentionDays,
            Reason = ResolveReason(hasMotionDetected, isArchived)
        };
    }

    private static int ResolveRetentionDays(
        bool hasMotionDetected,
        bool isArchived,
        int motionRetentionDays,
        int noMotionRetentionDays,
        int archivedRetentionDays)
    {
        if (isArchived)
        {
            return archivedRetentionDays;
        }

        return hasMotionDetected ? motionRetentionDays : noMotionRetentionDays;
    }

    private static string ResolveReason(bool hasMotionDetected, bool isArchived)
    {
        if (isArchived)
        {
            return "Archived video exceeded archived retention policy.";
        }

        return hasMotionDetected
            ? "Motion video exceeded motion retention policy."
            : "No-motion video exceeded short retention policy.";
    }
}
