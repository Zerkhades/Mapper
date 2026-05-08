using Hangfire;
using Mapper.Application.Features.Retention.Commands;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

namespace Mapper.Infrastructure.BackgroundJobs;

public class CleanupArchiveRetentionJob
{
    private readonly IMediator _mediator;
    private readonly IOptions<ArchiveRetentionCleanupJobOptions> _options;
    private readonly ILogger<CleanupArchiveRetentionJob> _logger;

    public CleanupArchiveRetentionJob(
        IMediator mediator,
        IOptions<ArchiveRetentionCleanupJobOptions> options,
        ILogger<CleanupArchiveRetentionJob> logger)
    {
        _mediator = mediator;
        _options = options;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 2)]
    public async Task Execute(CancellationToken ct = default)
    {
        var options = _options.Value;
        if (!options.Enabled)
        {
            _logger.LogInformation("Archive retention cleanup job is disabled");
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            var result = await _mediator.Send(new CleanupArchiveRetentionCommand(
                Now: DateTimeOffset.UtcNow,
                MotionVideoRetentionDays: options.MotionVideoRetentionDays,
                NoMotionVideoRetentionDays: options.NoMotionVideoRetentionDays,
                ArchivedVideoRetentionDays: options.ArchivedVideoRetentionDays,
                Take: options.Take,
                DryRun: options.DryRun,
                Confirm: options.Confirm), ct);

            stopwatch.Stop();
            BackgroundJobMetrics.RecordArchiveRetentionCleanupSuccess(result, stopwatch.Elapsed);

            _logger.LogInformation(
                "Archive retention cleanup completed. DryRun={DryRun}, Confirmed={Confirmed}, Candidates={CandidateCount}, Deleted={DeletedCount}, ReclaimableBytes={ReclaimableBytes}, DurationMs={DurationMs}",
                result.DryRun,
                result.Confirmed,
                result.CandidateCount,
                result.DeletedCount,
                result.ReclaimableBytes,
                stopwatch.ElapsedMilliseconds);
        }
        catch
        {
            stopwatch.Stop();
            BackgroundJobMetrics.RecordArchiveRetentionCleanupFailure(
                options.DryRun,
                options.Confirm,
                stopwatch.Elapsed);
            throw;
        }
    }
}
