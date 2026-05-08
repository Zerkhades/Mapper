using Mapper.Application.Features.Retention.DTOs;
using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Mapper.Infrastructure.BackgroundJobs;

public static class BackgroundJobMetrics
{
    public const string MeterName = "Mapper.Infrastructure.BackgroundJobs";

    private static readonly Meter Meter = new(MeterName, "1.0.0");

    private static readonly Counter<long> JobRuns = Meter.CreateCounter<long>(
        "mapper.background_job.runs",
        unit: "{run}",
        description: "Background job executions grouped by job name and outcome.");

    private static readonly Histogram<double> JobDuration = Meter.CreateHistogram<double>(
        "mapper.background_job.duration",
        unit: "ms",
        description: "Background job execution duration in milliseconds.");

    private static readonly Counter<long> RetentionCleanupCandidates = Meter.CreateCounter<long>(
        "mapper.retention.cleanup.candidates",
        unit: "{video}",
        description: "Archive retention cleanup candidate videos.");

    private static readonly Counter<long> RetentionCleanupDeletedVideos = Meter.CreateCounter<long>(
        "mapper.retention.cleanup.deleted_videos",
        unit: "{video}",
        description: "Archive retention cleanup deleted videos.");

    private static readonly Counter<long> RetentionCleanupReclaimableBytes = Meter.CreateCounter<long>(
        "mapper.retention.cleanup.reclaimable_bytes",
        unit: "By",
        description: "Archive retention cleanup reclaimable storage bytes.");

    public static void RecordArchiveRetentionCleanupSuccess(
        ArchiveRetentionCleanupResultDto result,
        TimeSpan duration)
    {
        var tags = CreateArchiveRetentionTags("success", result.DryRun, result.Confirmed);

        JobRuns.Add(1, tags);
        JobDuration.Record(duration.TotalMilliseconds, tags);
        RetentionCleanupCandidates.Add(result.CandidateCount, tags);
        RetentionCleanupDeletedVideos.Add(result.DeletedCount, tags);
        RetentionCleanupReclaimableBytes.Add(result.ReclaimableBytes, tags);
    }

    public static void RecordArchiveRetentionCleanupFailure(
        bool dryRun,
        bool confirmed,
        TimeSpan duration)
    {
        var tags = CreateArchiveRetentionTags("failure", dryRun, confirmed);

        JobRuns.Add(1, tags);
        JobDuration.Record(duration.TotalMilliseconds, tags);
    }

    private static TagList CreateArchiveRetentionTags(
        string outcome,
        bool dryRun,
        bool confirmed)
    {
        return new TagList
        {
            { "job.name", "archive_retention_cleanup" },
            { "job.outcome", outcome },
            { "retention.dry_run", dryRun },
            { "retention.confirmed", confirmed }
        };
    }
}
