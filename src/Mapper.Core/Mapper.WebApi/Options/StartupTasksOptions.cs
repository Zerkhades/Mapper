namespace Mapper.WebApi.Options;

public class StartupTasksOptions
{
    public const string SectionName = "StartupTasks";

    public bool ApplyMigrations { get; set; }

    public bool SeedDatabase { get; set; }

    public bool EnsureS3Bucket { get; set; }

    public int MigrateAttempts { get; set; } = 10;
}
