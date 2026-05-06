namespace Mapper.Infrastructure.BackgroundJobs;

public class ArchiveRetentionCleanupJobOptions
{
    public const string SectionName = "Retention:ArchiveCleanup";

    public bool Enabled { get; set; }

    public string Cron { get; set; } = "0 3 * * *";

    public int MotionVideoRetentionDays { get; set; } = 90;

    public int NoMotionVideoRetentionDays { get; set; } = 7;

    public int ArchivedVideoRetentionDays { get; set; } = 365;

    public int Take { get; set; } = 100;

    public bool DryRun { get; set; } = true;

    public bool Confirm { get; set; }
}
