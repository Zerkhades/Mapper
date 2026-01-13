using Amazon.S3;
using Amazon.S3.Model;
using Mapper.Application.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mapper.Infrastructure.Storage.S3;

public class S3ObjectStorage : IS3ObjectStorage
{
    private readonly IAmazonS3 _s3;
    private readonly string _bucket;

    public S3ObjectStorage(IAmazonS3 s3, IConfiguration cfg)
    {
        _s3 = s3;
        _bucket = cfg["S3:Bucket"] ?? "mapper";
    }

    public async Task<string> PutAsync(string key, Stream content, string contentType, CancellationToken ct)
    {
        if (content.CanSeek) content.Position = 0;

        await _s3.PutObjectAsync(new PutObjectRequest
        {
            BucketName = _bucket,
            Key = key,
            InputStream = content,
            ContentType = contentType
        }, ct);

        return key;
    }
}
