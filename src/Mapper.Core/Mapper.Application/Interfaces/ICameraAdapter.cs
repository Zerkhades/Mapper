using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public record CameraStatus(bool IsOnline, int? RttMs = null, string? Message = null);

    public record CameraSnapshot(byte[] Bytes, string ContentType, string FileName);

    public record CameraVideo(byte[] Bytes, string ContentType, string FileName, TimeSpan Duration);

    public record MotionDetectionResult(
        bool HasMotion,
        double MotionPercentage, // 0-100
        List<(int x, int y, int width, int height)>? MotionAreas = null);

    public interface ICameraAdapter
    {
        Task<CameraStatus> GetStatusAsync(string? streamUrl, CancellationToken ct);
        Task<CameraSnapshot?> TryGetSnapshotAsync(string? streamUrl, CancellationToken ct);
        
        // Video and motion detection
        Task<CameraVideo?> TryGetVideoAsync(string? streamUrl, TimeSpan duration, CancellationToken ct);
        Task<MotionDetectionResult?> TryDetectMotionAsync(string? streamUrl, byte[] frameData, CancellationToken ct);
        Task<CameraSnapshot?> TryGetSnapshotWithZoomAsync(
            string? streamUrl,
            double zoomLevel, // 1.0 = 100%, 2.0 = 200%
            int? centerX = null,
            int? centerY = null,
            CancellationToken ct = default);
    }
}
