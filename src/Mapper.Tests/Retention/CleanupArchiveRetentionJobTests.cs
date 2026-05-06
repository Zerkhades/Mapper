using Mapper.Application.Features.Retention.Commands;
using Mapper.Application.Features.Retention.DTOs;
using Mapper.Infrastructure.BackgroundJobs;
using MediatR;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

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
    }
}
