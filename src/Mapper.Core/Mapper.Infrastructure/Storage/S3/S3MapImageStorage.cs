using Amazon.S3;
using Amazon.S3.Model;
using Mapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mapper.Infrastructure.Storage.S3
{
    public class S3MapImageStorage : IMapImageStorage
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;

        public S3MapImageStorage(IAmazonS3 s3, IConfiguration cfg)
        {
            _s3 = s3;
            _bucket = cfg["S3:Bucket"]!;
        }

        public async Task<string> SaveAsync(Stream content, string fileName, string contentType, CancellationToken ct)
        {
            var key = $"maps/{Guid.NewGuid()}_{fileName}";

            if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
            {
                contentType = DetectContentType(fileName);
            }

            if (content.CanSeek) content.Position = 0;

            var req = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                InputStream = content,
                ContentType = contentType
            };

            await _s3.PutObjectAsync(req, ct);
            return key;
        }

        private static string DetectContentType(string fileName)
        {
            var ext = Path.GetExtension(fileName)?.ToLowerInvariant();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".bmp" => "image/bmp",
                ".svg" => "image/svg+xml",
                _ => "application/octet-stream"
            };
        }
    }
}
