namespace Mapper.Application.Features.Retention.DTOs;

public class ArchiveRetentionCleanupResultDto
{
    public DateTimeOffset ExecutedAt { get; set; }
    public bool DryRun { get; set; }
    public bool Confirmed { get; set; }
    public int CandidateCount { get; set; }
    public int DeletedCount { get; set; }
    public long ReclaimableBytes { get; set; }
    public List<ArchiveRetentionCandidateDto> Candidates { get; set; } = [];
}
