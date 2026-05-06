namespace Mapper.Application.Features.Analytics.DTOs;

public class OperatorDashboardDto
{
    public DateTimeOffset PeriodStart { get; set; }
    public DateTimeOffset PeriodEnd { get; set; }
    public int TotalCameras { get; set; }
    public int OnlineCameras { get; set; }
    public int OfflineCameras { get; set; }
    public int UnknownStatusCameras { get; set; }
    public int MotionAlerts { get; set; }
    public int UnresolvedMotionAlerts { get; set; }
    public int HighSeverityUnresolvedAlerts { get; set; }
    public int ArchivedVideos { get; set; }
    public double AverageMotionPercentage { get; set; }
    public List<CameraActivityDto> TopActiveCameras { get; set; } = [];
    public List<CameraAnomalyDto> Anomalies { get; set; } = [];
    public List<CameraStatusSummaryDto> ProblemCameras { get; set; } = [];
}

public class CameraActivityDto
{
    public Guid CameraMarkId { get; set; }
    public string CameraTitle { get; set; } = default!;
    public int MotionAlerts { get; set; }
    public int UnresolvedAlerts { get; set; }
    public double AverageMotionPercentage { get; set; }
}

public class CameraAnomalyDto
{
    public Guid CameraMarkId { get; set; }
    public string CameraTitle { get; set; } = default!;
    public int CurrentAlertCount { get; set; }
    public double BaselineAverageAlertCount { get; set; }
    public double ZScore { get; set; }
    public double ActivityRatio { get; set; }
    public string Reason { get; set; } = default!;
}

public class CameraStatusSummaryDto
{
    public Guid CameraMarkId { get; set; }
    public string CameraTitle { get; set; } = default!;
    public bool? IsOnline { get; set; }
    public string Status { get; set; } = default!;
    public DateTimeOffset? ChangedAt { get; set; }
    public string? Reason { get; set; }
    public int? ResponseTimeMs { get; set; }
}
