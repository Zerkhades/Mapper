using Amazon.S3;
using Amazon.S3.Util;
using Mapper.WebApi.Options;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Mapper.WebApi.HealthChecks;

public class S3HealthCheck : IHealthCheck
{
    private readonly IAmazonS3 _s3;
    private readonly IOptionsMonitor<S3Options> _options;

    public S3HealthCheck(IAmazonS3 s3, IOptionsMonitor<S3Options> options)
    {
        _s3 = s3;
        _options = options;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var bucket = _options.CurrentValue.Bucket;
        if (string.IsNullOrWhiteSpace(bucket))
        {
            return HealthCheckResult.Degraded("S3 bucket is not configured.");
        }

        try
        {
            var exists = await AmazonS3Util.DoesS3BucketExistV2Async(_s3, bucket);
            return exists
                ? HealthCheckResult.Healthy("S3 bucket is available.")
                : HealthCheckResult.Unhealthy("S3 bucket does not exist.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("S3 health check failed.", ex);
        }
    }
}
