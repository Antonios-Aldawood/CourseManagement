using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CourseManagement.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;

namespace CourseManagement.Infrastructure.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
        }

        public async Task <T?> GetData<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);

            if (data is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(data);
        }
    }
}
