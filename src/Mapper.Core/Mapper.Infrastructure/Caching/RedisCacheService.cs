using Mapper.Application.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Mapper.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _db;
        private static readonly JsonSerializerOptions Opt = new(JsonSerializerDefaults.Web);

        public RedisCacheService(IConnectionMultiplexer mux) => _db = mux.GetDatabase();

        public async Task<T?> GetAsync<T>(string key, CancellationToken ct)
        {
            var value = await _db.StringGetAsync(key);
            if (value.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(value.ToString(), Opt);
        }

        public Task SetAsync<T>(string key, T value, TimeSpan ttl, CancellationToken ct)
        {
            var json = JsonSerializer.Serialize(value, Opt);
            return _db.StringSetAsync(key, json, ttl);
        }

        public Task RemoveAsync(string key, CancellationToken ct)
            => _db.KeyDeleteAsync(key);
    }
}
