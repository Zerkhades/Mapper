namespace Mapper.Domain;

public enum CameraStatusReason
{
    Unknown = 0,
    NetworkConnected = 1,
    NetworkDisconnected = 2,
    Reboot = 3,
    PowerOn = 4,
    PowerOff = 5,
    ManualReset = 6,
    ConfigurationChanged = 7,
    FirmwareUpdate = 8,
    NetworkTimeout = 9,
    DNSResolutionFailed = 10,
    Unauthorized = 11,
    CertificateError = 12,
    HardwareError = 13,
    OutOfMemory = 14,
    ThermalShutdown = 15
}

public class CameraStatusHistory
{
    public Guid Id { get; private init; } = Guid.NewGuid();
    public Guid CameraMarkId { get; private set; }
    public bool IsOnline { get; private set; }
    public CameraStatusReason Reason { get; private set; }
    public DateTimeOffset ChangedAt { get; private set; }
    public TimeSpan? DurationSinceLastChange { get; private set; }
    public string? Details { get; private set; }
    public int? ResponseTimeMs { get; private set; }

    private CameraStatusHistory() { } // EF

    public CameraStatusHistory(
        Guid cameraMarkId,
        bool isOnline,
        CameraStatusReason reason,
        string? details = null,
        int? responseTimeMs = null)
    {
        CameraMarkId = cameraMarkId;
        IsOnline = isOnline;
        Reason = reason;
        Details = details;
        ResponseTimeMs = responseTimeMs;
        ChangedAt = DateTimeOffset.UtcNow;
    }

    public void SetDuration(TimeSpan duration) => DurationSinceLastChange = duration;
}
