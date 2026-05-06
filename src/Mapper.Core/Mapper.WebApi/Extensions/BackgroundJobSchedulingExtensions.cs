using Hangfire;
using Mapper.Infrastructure.BackgroundJobs;
using Serilog;

namespace Mapper.WebApi.Extensions;

public static class BackgroundJobSchedulingExtensions
{
    public static void ScheduleCameraJobs(this IApplicationBuilder app, IConfiguration configuration)
    {
        try
        {
            var motionCron = configuration["Camera:Jobs:MotionCron"] ?? "*/5 * * * *";
            var recordCron = configuration["Camera:Jobs:VideoCron"] ?? "*/30 * * * *";
            var statusCron = configuration["Camera:Jobs:StatusCron"] ?? "*/1 * * * *";
            var snapshotCron = configuration["Camera:Jobs:SnapshotCron"] ?? "*/1 * * * *";
            var archiveCleanupEnabled = configuration.GetValue<bool>("Retention:ArchiveCleanup:Enabled");
            var archiveCleanupCron = configuration["Retention:ArchiveCleanup:Cron"] ?? "0 3 * * *";

            RecurringJob.AddOrUpdate<DetectCameraMotionJob>(
                "detect-camera-motion",
                j => j.Execute(default),
                motionCron);

            RecurringJob.AddOrUpdate<RecordCameraVideoJob>(
                "record-camera-video",
                j => j.Execute(default),
                recordCron);

            RecurringJob.AddOrUpdate<PollCameraStatusAndLogHistoryJob>(
                "poll-camera-status",
                j => j.Execute(default),
                statusCron);

            RecurringJob.AddOrUpdate<FetchCameraSnapshotsJob>(
                "fetch-camera-snapshots",
                j => j.Execute(default),
                snapshotCron);

            if (archiveCleanupEnabled)
            {
                RecurringJob.AddOrUpdate<CleanupArchiveRetentionJob>(
                    "cleanup-archive-retention",
                    j => j.Execute(default),
                    archiveCleanupCron);
            }
            else
            {
                RecurringJob.RemoveIfExists("cleanup-archive-retention");
            }
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to schedule camera background jobs. Check database connection settings.");
        }
    }
}
