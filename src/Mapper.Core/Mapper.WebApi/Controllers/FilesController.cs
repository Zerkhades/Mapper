using Amazon.S3;
using Amazon.S3.Model;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Serilog;

namespace Mapper.WebApi.Controllers;

[ApiController]
[AllowAnonymous]
[ApiVersionNeutral]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IAmazonS3 _s3;
    private readonly IConfiguration _cfg;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider = new();

    public FilesController(IAmazonS3 s3, IConfiguration cfg)
    {
        _s3 = s3;
        _cfg = cfg;
    }

    [AllowAnonymous]
    [HttpGet("{**key}")]
    public async Task<IActionResult> Get([FromRoute] string key, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(key))
            return BadRequest("Empty key");

        // MVP-ограничение
        if (!key.StartsWith("maps/", StringComparison.OrdinalIgnoreCase))
            return Forbid();

        var bucket = _cfg["S3:Bucket"] ?? "mapper";

        try
        {
            var obj = await _s3.GetObjectAsync(new GetObjectRequest
            {
                BucketName = bucket,
                Key = key
            }, ct);

            // 1) ContentType из S3
            var contentType = obj.Headers.ContentType;

            // 2) Если он пустой или application/octet-stream — берём по расширению
            if (string.IsNullOrWhiteSpace(contentType) || contentType == "application/octet-stream")
            {
                if (!_contentTypeProvider.TryGetContentType(key, out contentType))
                    contentType = "application/octet-stream";
            }

            Response.Headers.CacheControl = "public, max-age=60";

            return File(obj.ResponseStream, contentType);
        }
        catch (AmazonS3Exception ex)
        {
            Log.Error(ex, "S3 GetObject failed. Bucket={Bucket} Key={Key} Status={Status}", bucket, key, ex.StatusCode);

            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
                return NotFound();

            return StatusCode((int)ex.StatusCode);
        }
    }
}
