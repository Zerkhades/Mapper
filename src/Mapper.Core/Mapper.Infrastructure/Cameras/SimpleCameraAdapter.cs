using Mapper.Application.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Globalization;
using System.Net.Sockets;
using System.Buffers.Binary;
using System.IO;

namespace Mapper.Infrastructure.Cameras
{
    public class SimpleCameraAdapter : ICameraAdapter
    {
        private const int SnapshotTimeoutSeconds = 5;
        private const int ProcessTimeoutSeconds = 15;
        private static readonly ConcurrentDictionary<string, byte[]> LastFrames = new(StringComparer.OrdinalIgnoreCase);
        private readonly ILogger<SimpleCameraAdapter> _logger;

        public SimpleCameraAdapter(ILogger<SimpleCameraAdapter> logger) => _logger = logger;

        public async Task<CameraStatus> GetStatusAsync(string? streamUrl, CancellationToken ct)
        {
            var sw = Stopwatch.StartNew();
            var online = await IsOnlineAsync(streamUrl, ct);
            sw.Stop();

            return new CameraStatus(
                IsOnline: online,
                RttMs: online ? (int)sw.ElapsedMilliseconds : null,
                Message: online ? "Online" : "Offline"
            );
        }

        public async Task<bool> IsOnlineAsync(string? streamUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(streamUrl)) return false;

            if (!Uri.TryCreate(streamUrl, UriKind.Absolute, out var uri)) return false;

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

        public async Task<CameraSnapshot?> TryGetSnapshotAsync(string? streamUrl, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(streamUrl)) return null;

            var outputPath = Path.Combine(Path.GetTempPath(), $"snapshot_{Guid.NewGuid():N}.png");

            try
            {
                var args = $"-y -hide_banner -loglevel error -rtsp_transport tcp -i \"{streamUrl}\" -frames:v 1 -update 1 -fps_mode vfr -f image2 \"{outputPath}\"";
                var result = await RunProcessAsync(GetFfmpegPath(), args, ct);

                var exists = File.Exists(outputPath);

                if (result.ExitCode != 0 || !exists)
                {
                    _logger.LogWarning("FFmpeg snapshot failed. ExitCode={ExitCode}, Exists={Exists}, Output={OutputPath}, Stream={StreamUrl}, Error={Error}",
                        result.ExitCode, exists, outputPath, streamUrl, result.StandardError);
                    return null;
                }

                var bytes = await File.ReadAllBytesAsync(outputPath, ct);
                return new CameraSnapshot(bytes, "image/png", Path.GetFileName(outputPath));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Snapshot capture error for stream {StreamUrl}", streamUrl);
                return null;
            }
            finally
            {
                TryDelete(outputPath);
            }
        }

        public async Task<CameraVideo?> TryGetVideoAsync(string? streamUrl, TimeSpan duration, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(streamUrl)) return null;

            var outputPath = Path.Combine(Path.GetTempPath(), $"video_{Guid.NewGuid():N}.mp4");
            var durationSec = duration.TotalSeconds.ToString(CultureInfo.InvariantCulture);

            try
            {
                var args = $"-y -hide_banner -loglevel error -rtsp_transport tcp -i \"{streamUrl}\" -t {durationSec} -an -c:v libx264 -preset veryfast -crf 28 \"{outputPath}\"";
                var result = await RunProcessAsync(GetFfmpegPath(), args, ct);

                var exists = File.Exists(outputPath);
                if (result.ExitCode != 0 || !exists)
                {
                    _logger.LogWarning("FFmpeg video capture failed. ExitCode={ExitCode}, Exists={Exists}, Output={OutputPath}, Stream={StreamUrl}, Error={Error}",
                        result.ExitCode, exists, outputPath, streamUrl, result.StandardError);
                    return null;
                }

                var bytes = await File.ReadAllBytesAsync(outputPath, ct);
                return new CameraVideo(bytes, "video/mp4", Path.GetFileName(outputPath), duration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Video capture error for stream {StreamUrl}", streamUrl);
                return null;
            }
            finally
            {
                TryDelete(outputPath);
            }
        }

        public Task<MotionDetectionResult?> TryDetectMotionAsync(string? streamUrl, byte[] frameData, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(streamUrl) || frameData.Length == 0)
                return Task.FromResult<MotionDetectionResult?>(new MotionDetectionResult(false, 0));

            var key = streamUrl.Trim();
            if (!LastFrames.TryGetValue(key, out var lastFrame))
            {
                LastFrames[key] = frameData;
                return Task.FromResult<MotionDetectionResult?>(new MotionDetectionResult(false, 0));
            }

            var length = Math.Min(frameData.Length, lastFrame.Length);
            if (length == 0)
                return Task.FromResult<MotionDetectionResult?>(new MotionDetectionResult(false, 0));

            var step = Math.Max(1, length / 50_000);
            long diffCount = 0;
            long sampleCount = 0;

            for (var i = 0; i < length; i += step)
            {
                sampleCount++;
                if (Math.Abs(frameData[i] - lastFrame[i]) > 20)
                    diffCount++;
            }

            var motionPercentage = sampleCount == 0 ? 0 : diffCount * 100d / sampleCount;
            var hasMotion = motionPercentage > 5;

            LastFrames[key] = frameData;

            return Task.FromResult<MotionDetectionResult?>(new MotionDetectionResult(hasMotion, motionPercentage));
        }

        public async Task<CameraSnapshot?> TryGetSnapshotWithZoomAsync(
            string? streamUrl,
            double zoomLevel,
            int? centerX = null,
            int? centerY = null,
            CancellationToken ct = default)
        {
            var snapshot = await TryGetSnapshotAsync(streamUrl, ct);
            if (snapshot is null) return null;

            if (zoomLevel <= 1.01)
                return snapshot;

            if (!TryReadPngDimensions(snapshot.Bytes, out var width, out var height))
                return snapshot;

            var cropWidth = Math.Max(1, (int)(width / zoomLevel));
            var cropHeight = Math.Max(1, (int)(height / zoomLevel));

            var cx = Math.Clamp(centerX ?? width / 2, 0, width);
            var cy = Math.Clamp(centerY ?? height / 2, 0, height);

            var x = Math.Clamp(cx - cropWidth / 2, 0, width - cropWidth);
            var y = Math.Clamp(cy - cropHeight / 2, 0, height - cropHeight);

            var inputPath = Path.Combine(Path.GetTempPath(), $"zoom_in_{Guid.NewGuid():N}.png");
            var outputPath = Path.Combine(Path.GetTempPath(), $"zoom_out_{Guid.NewGuid():N}.png");

            try
            {
                await File.WriteAllBytesAsync(inputPath, snapshot.Bytes, ct);
                var args = $"-y -hide_banner -loglevel error -i \"{inputPath}\" -vf \"crop={cropWidth}:{cropHeight}:{x}:{y},scale={width}:{height}\" \"{outputPath}\"";
                var result = await RunProcessAsync(GetFfmpegPath(), args, ct);

                var exists = File.Exists(outputPath);
                if (result.ExitCode != 0 || !exists)
                {
                    _logger.LogWarning("FFmpeg zoom snapshot failed. ExitCode={ExitCode}, Exists={Exists}, Output={OutputPath}, Stream={StreamUrl}, Error={Error}",
                        result.ExitCode, exists, outputPath, streamUrl, result.StandardError);
                    return snapshot;
                }

                var bytes = await File.ReadAllBytesAsync(outputPath, ct);
                return new CameraSnapshot(bytes, "image/png", $"snapshot_zoom_{zoomLevel:F1}x.png");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Zoom snapshot error for stream {StreamUrl}", streamUrl);
                return snapshot;
            }
            finally
            {
                TryDelete(inputPath);
                TryDelete(outputPath);
            }
        }

        private static string GetFfmpegPath()
        {
            var path = Environment.GetEnvironmentVariable("FFMPEG_PATH");
            return string.IsNullOrWhiteSpace(path) ? "ffmpeg" : path;
        }

        private record ProcessResult(int ExitCode, string StandardError);

        private static async Task<ProcessResult> RunProcessAsync(string fileName, string args, CancellationToken ct)
        {
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = fileName,
                    Arguments = args,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process.Start();

            var stdOutTask = process.StandardOutput.ReadToEndAsync(ct);
            var stdErrTask = process.StandardError.ReadToEndAsync(ct);
            var waitTask = process.WaitForExitAsync(ct);

            var completed = await Task.WhenAny(waitTask, Task.Delay(TimeSpan.FromSeconds(ProcessTimeoutSeconds), ct));
            if (completed != waitTask)
            {
                try { process.Kill(); } catch { }
                return new ProcessResult(-1, "FFmpeg process timed out");
            }

            var stdErr = await stdErrTask;
            await stdOutTask;
            return new ProcessResult(process.ExitCode, stdErr);
        }

        private static bool TryReadPngDimensions(byte[] bytes, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (bytes.Length < 24)
                return false;

            if (bytes[0] != 137 || bytes[1] != 80 || bytes[2] != 78 || bytes[3] != 71)
                return false;

            width = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(16, 4));
            height = BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan(20, 4));

            return width > 0 && height > 0;
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch { }
        }
    }
}
