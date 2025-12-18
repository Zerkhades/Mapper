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
        public async Task<bool> IsOnlineAsync(string? streamUrl, CancellationToken ct)
        {
            // MVP: если HTTP/HTTPS — можно сделать HEAD/GET
            // если RTSP — хотя бы проверить, что host:port доступен (обычно 554)
            if (string.IsNullOrWhiteSpace(streamUrl)) return false;

            if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out var uri)) return false;

            var port = uri.Port > 0 ? uri.Port : (uri.Scheme == "rtsp" ? 554 : 80);

            try
            {
                using var client = new TcpClient();
                var connectTask = client.ConnectAsync(uri.Host, port);
                var completed = await Task.WhenAny(connectTask, Task.Delay(TimeSpan.FromSeconds(2), ct));
                return completed == connectTask && client.Connected;
            }
            catch { return false; }
        }
    }
}
