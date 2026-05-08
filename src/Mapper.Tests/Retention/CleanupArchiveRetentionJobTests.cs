using Mapper.Application.Features.Retention.Commands;
using Mapper.Application.Features.Retention.DTOs;
using Mapper.Infrastructure.BackgroundJobs;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;
using System.Diagnostics.Metrics;

namespace Mapper.Tests.Retention;

public class CleanupArchiveRetentionJobTests
{
    [Fact]
    public async Task Execute_WhenDisabled_ShouldNotSendCleanupCommand()
    {
        // Arrange
        var mediator = new Mock<IMediator>();
        var job = new CleanupArchiveRetentionJob(
            mediator.Object,
            Options.Create(new ArchiveRetentionCleanupJobOptions { Enabled = false }),
            NullLogger<CleanupArchiveRetentionJob>.Instance);

        // Act
        await job.Execute(CancellationToken.None);

        // Assert
        mediator.Verify(x => x.Send(It.IsAny<CleanupArchiveRetentionCommand>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Execute_WhenEnabled_ShouldSendConfiguredCleanupCommand()
    {
        // Arrange
        CleanupArchiveRetentionCommand? sentCommand = null;
        var measurements = new List<MetricMeasurement>();
        using var listener = CreateMetricsListener(measurements);
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(x => x.Send(It.IsAny<CleanupArchiveRetentionCommand>(), It.IsAny<CancellationToken>()))
            .Callback<object, CancellationToken>((request, _) => sentCommand = (CleanupArchiveRetentionCommand)request)
            .ReturnsAsync(new ArchiveRetentionCleanupResultDto
            {
                DryRun = true,
                CandidateCount = 2,
                ReclaimableBytes = 1024
            });

        var options = Options.Create(new ArchiveRetentionCleanupJobOptions
        {
            Enabled = true,
            MotionVideoRetentionDays = 30,
            NoMotionVideoRetentionDays = 3,
            ArchivedVideoRetentionDays = 180,
            Take = 25,
            DryRun = true,
            Confirm = false
        });

        var job = new CleanupArchiveRetentionJob(
            mediator.Object,
            options,
            NullLogger<CleanupArchiveRetentionJob>.Instance);

        // Act
        await job.Execute(CancellationToken.None);

        // Assert
        Assert.NotNull(sentCommand);
        Assert.Equal(30, sentCommand.MotionVideoRetentionDays);
        Assert.Equal(3, sentCommand.NoMotionVideoRetentionDays);
        Assert.Equal(180, sentCommand.ArchivedVideoRetentionDays);
        Assert.Equal(25, sentCommand.Take);
        Assert.True(sentCommand.DryRun);
        Assert.False(sentCommand.Confirm);
        Assert.Contains(measurements, x =>
            x.InstrumentName == "mapper.background_job.runs"
            && x.Value == 1
            && x.Tags["job.name"]?.ToString() == "archive_retention_cleanup"
            && x.Tags["job.outcome"]?.ToString() == "success"
            && x.Tags["retention.dry_run"] is true);
        Assert.Contains(measurements, x =>
            x.InstrumentName == "mapper.retention.cleanup.candidates"
            && x.Value == 2);
        Assert.Contains(measurements, x =>
            x.InstrumentName == "mapper.retention.cleanup.reclaimable_bytes"
            && x.Value == 1024);
    }

    [Fact]
    public async Task Execute_WhenCleanupFails_ShouldRecordFailureMetric()
    {
        // Arrange
        var measurements = new List<MetricMeasurement>();
        using var listener = CreateMetricsListener(measurements);
        var mediator = new Mock<IMediator>();
        mediator
            .Setup(x => x.Send(It.IsAny<CleanupArchiveRetentionCommand>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Cleanup failed."));

        var job = new CleanupArchiveRetentionJob(
            mediator.Object,
            Options.Create(new ArchiveRetentionCleanupJobOptions
            {
                Enabled = true,
                DryRun = false,
                Confirm = true
            }),
            NullLogger<CleanupArchiveRetentionJob>.Instance);

        // Act
        await Assert.ThrowsAsync<InvalidOperationException>(() => job.Execute(CancellationToken.None));

        // Assert
        Assert.Contains(measurements, x =>
            x.InstrumentName == "mapper.background_job.runs"
            && x.Value == 1
            && x.Tags["job.name"]?.ToString() == "archive_retention_cleanup"
            && x.Tags["job.outcome"]?.ToString() == "failure"
            && x.Tags["retention.dry_run"] is false
            && x.Tags["retention.confirmed"] is true);
    }

    private static MeterListener CreateMetricsListener(List<MetricMeasurement> measurements)
    {
        var listener = new MeterListener
        {
            InstrumentPublished = (instrument, meterListener) =>
            {
                if (instrument.Meter.Name == BackgroundJobMetrics.MeterName)
                {
                    meterListener.EnableMeasurementEvents(instrument);
                }
            }
        };

        listener.SetMeasurementEventCallback<long>((instrument, value, tags, _) =>
        {
            measurements.Add(new MetricMeasurement(instrument.Name, value, CopyTags(tags)));
        });
        listener.SetMeasurementEventCallback<double>((instrument, value, tags, _) =>
        {
            measurements.Add(new MetricMeasurement(instrument.Name, value, CopyTags(tags)));
        });
        listener.Start();

        return listener;
    }

    private static Dictionary<string, object?> CopyTags(ReadOnlySpan<KeyValuePair<string, object?>> tags)
    {
        var result = new Dictionary<string, object?>();
        foreach (var tag in tags)
        {
            result[tag.Key] = tag.Value;
        }

        return result;
    }

    private sealed record MetricMeasurement(
        string InstrumentName,
        double Value,
        Dictionary<string, object?> Tags);
}
