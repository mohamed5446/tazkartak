using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Tazkartk.Application.Interfaces.External;

namespace Tazkartk.Infrastructure.Caching
{
    public class RedisService : ICachingService
    {
        private IDistributedCache _cache;

        public RedisService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public T? GetData<T>(string key)
        {
            var data = _cache?.GetString(key);
            if (data == null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(data);
        }

        public void SetData<T>(string Key, T Data, int? minutes)
        {

            var options = new DistributedCacheEntryOptions();
            if (minutes.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes.Value);
            }

            _cache.SetString(Key, JsonSerializer.Serialize(Data), options);

        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
