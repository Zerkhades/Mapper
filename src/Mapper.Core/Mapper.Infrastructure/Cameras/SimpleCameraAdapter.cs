using Mapper.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Mapper.Infrastructure.Cameras
{
    public class SimpleCameraAdapter : ICameraAdapter
    {
        public Task<CameraStatus> GetStatusAsync(string? streamUrl, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> IsOnlineAsync(string? streamUrl, CancellationToken ct)
        {
            // MVP: если HTTP/HTTPS — можно сделать HEAD/GET
            // если RTSP — хотя бы проверить, что host:port доступен (обычно 554)
            if (string.IsNullOrWhiteSpace(streamUrl)) return false;

            if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out var uri)) return false;

            // change 1935 to 554 later
            var port = uri.Port > 0 ? uri.Port : (uri.Scheme == "rtsp" ? 1935 : 80);

            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(uri.Host, port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(TimeSpan.FromSeconds(2), ct));
                return completed == connectTask && client.Connected;
            }
            catch { return false; }
        }

        public Task<CameraSnapshot?> TryGetSnapshotAsync(string? streamUrl, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
