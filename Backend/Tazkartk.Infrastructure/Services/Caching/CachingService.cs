using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using Tazkartk.Application.Interfaces;

namespace Tazkartk.Infrastructure.Caching
{
    public class CachingService : ICachingService
    {
        private IDistributedCache _cache;

        public CachingService(IDistributedCache cache)
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

        public void SetData<T>(string Key, T Data, int minutes)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes),
            };
            _cache.SetString(Key, JsonSerializer.Serialize(Data), options);

        }
        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
