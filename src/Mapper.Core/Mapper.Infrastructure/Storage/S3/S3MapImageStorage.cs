using Amazon.S3;
using Amazon.S3.Model;
using Mapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.StaticFiles;

namespace Mapper.Infrastructure.Storage.S3
{
    public class S3MapImageStorage : IMapImageStorage
    {
        private readonly IAmazonS3 _s3;
        private readonly string _bucket;
        private static readonly FileExtensionContentTypeProvider Ctp = new();

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
                if (!Ctp.TryGetContentType(fileName, out contentType))
                    contentType = "application/octet-stream";
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
    }
}
