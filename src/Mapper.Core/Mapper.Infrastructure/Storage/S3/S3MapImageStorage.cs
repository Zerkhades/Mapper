using Amazon.S3;
using Amazon.S3.Model;
using Mapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var req = new PutObjectRequest
            {
                BucketName = _bucket,
                Key = key,
                InputStream = content,
                ContentType = contentType
            };

            await _s3.PutObjectAsync(req, ct);
            return key; // в БД храним key
        }
    }
}
