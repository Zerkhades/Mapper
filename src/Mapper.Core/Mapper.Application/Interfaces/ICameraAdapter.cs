using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Application.Interfaces
{
    public record CameraStatus(bool IsOnline, int? RttMs = null, string? Message = null);

    public record CameraSnapshot(byte[] Bytes, string ContentType, string FileName);

    public interface ICameraAdapter
    {
        Task<CameraStatus> GetStatusAsync(string? streamUrl, CancellationToken ct);
        Task<CameraSnapshot?> TryGetSnapshotAsync(string? streamUrl, CancellationToken ct);
    }
}
