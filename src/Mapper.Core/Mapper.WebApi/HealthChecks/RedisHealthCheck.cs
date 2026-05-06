using Microsoft.Extensions.Diagnostics.HealthChecks;
using StackExchange.Redis;

namespace Mapper.WebApi.HealthChecks;

public class RedisHealthCheck : IHealthCheck
{
    private readonly IConnectionMultiplexer _connection;

    public RedisHealthCheck(IConnectionMultiplexer connection)
    {
        _connection = connection;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_connection.IsConnected)
            {
                return HealthCheckResult.Unhealthy("Redis connection is not available.");
            }

            await _connection.GetDatabase().PingAsync();
            return HealthCheckResult.Healthy("Redis connection is available.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Redis health check failed.", ex);
        }
    }
}
