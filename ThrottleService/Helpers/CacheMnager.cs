using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThrottleService.Helpers
{
    public static class CacheMnager
    {
        public static async Task CacheSaveAsync(this IDistributedCache source, string key, object content)
        {
            string _content = (content is string) ? (string)content :
              JsonConvert.SerializeObject(content);
            await source.SetAsync(key, Encoding.UTF8.GetBytes(_content));
        }

        static async Task<T> CacheGetAsync<T>(this IDistributedCache source, string key) where T : class
        {
            var cacheValue = await source.GetAsync(key);
            if (cacheValue == null)
            {
                return default(T);
            }
            var str = Encoding.UTF8.GetString(cacheValue);
            if (typeof(T) == typeof(string))
            {
                return str as T;
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        public static async Task<IEnumerable<T>> CacheGetListAsync<T>(this IDistributedCache source, string key) where T : class
        {
            var cacheValue = await source.CacheGetAsync<IEnumerable<T>>(key);
            if (cacheValue == null)
                return Enumerable.Empty<T>();
            return cacheValue as IEnumerable<T>;
        }
        static T CacheGet<T>(this IDistributedCache source, string key) where T : class
        {
            return source.CacheGetAsync<T>(key).Result;
        }

        public static IEnumerable<T> CacheGetList<T>(this IDistributedCache source, string key) where T : class
        {
            var cacheValue = source.CacheGet<IEnumerable<T>>(key);
            if (cacheValue == null)
                return Enumerable.Empty<T>();
            return cacheValue as IEnumerable<T>;
        }

        public static async Task<T> CacheFindAsync<T>(this IDistributedCache source, string key, Func<T, bool> predicate)
        {
            var list = await source.CacheGetAsync<IEnumerable<T>>(key);
            return list.SingleOrDefault(predicate);
        }

        public static async Task<IEnumerable<T>> CacheReplaceAsync<T>(this IDistributedCache source, string key, Func<T, bool> predicate, T newValue)
        {
            var list = await source.CacheGetAsync<IEnumerable<T>>(key);
            return list.Replace(newValue, predicate);
        }

        public static async Task<IEnumerable<T>> CacheRemoveAsync<T>(this IDistributedCache source, string key, Func<T, bool> predicate)
        {
            var list = await source.CacheGetAsync<IEnumerable<T>>(key);
            return list.RemoveAll(predicate);
        }
    }
}
