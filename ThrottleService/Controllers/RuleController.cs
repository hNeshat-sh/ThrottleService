using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;
using Newtonsoft.Json;
using ThrottleService.Resources;

namespace ThrottleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IDistributedCache _distributedCache;
        public RuleController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        // GET: api/Rule
        [HttpGet]
        public IEnumerable<Rule> Get()
        {
            var rules = CacheGet<IEnumerable<Rule>>("Rules");
            return rules.ToArray();
        }

        //GET: api/Rule/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Rule>> Get(int id)
        {
            var rule = await CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (rule == null)
                return NotFound();
            return rule;
        }

        // POST: api/Rule
        [HttpPost]
        public async Task<ActionResult<Rule>> Post(Rule rule)
        {
            var rules = (await CacheGetAsync<IEnumerable<Rule>>("Rules")).ToList();
            rules.Add(rule);
            await CacheSaveAsync("Rules", rules.ToArray());
            return CreatedAtAction(nameof(Get), new { id = rule.Id }, rule);
        }

        // PUT: api/Rule/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, Rule rule)
        {
            if (id != rule.Id)
            {
                return BadRequest();
            }
            var _rule = await CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (_rule == null)
                return NotFound();
            var rules = await CacheReplaceAsync<Rule>("Rules", a => a.Id == id, rule);
            await CacheSaveAsync("Rules", rules.ToArray());
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rule>> Delete(int id)
        {
            var rule = await CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (rule == null)
            {
                return NotFound();
            }
            var rules = await CacheRemoveAsync<Rule>("Rules", a => a.Id == id);
            await CacheSaveAsync("Rules", rules.ToArray());
            return rule;
        }

        public async Task CacheSaveAsync(string key, object content)
        {
            string _content = (content is string) ? (string)content :
              JsonConvert.SerializeObject(content);
            await _distributedCache.SetAsync(key, Encoding.UTF8.GetBytes(_content));
        }

        public async Task<T> CacheGetAsync<T>(string key) where T : class
        {
            var cacheValue = await _distributedCache.GetAsync(key);
            if (cacheValue == null)
            {
                return null;
            }
            var str = Encoding.UTF8.GetString(cacheValue);
            if (typeof(T) == typeof(string))
            {
                return str as T;
            }
            return JsonConvert.DeserializeObject<T>(str);
        }

        public T CacheGet<T>(string key) where T : class
        {
            return CacheGetAsync<T>(key).Result;
        }

        public async Task<T> CacheFindAsync<T>(string key, Func<T, bool> predicate)
        {
            var list = await CacheGetAsync<IEnumerable<T>>(key);
            return list.SingleOrDefault(predicate);
        }

        public async Task<IEnumerable<T>> CacheReplaceAsync<T>(string key, Func<T, bool> predicate, T newValue)
        {
            var list = await CacheGetAsync<IEnumerable<T>>(key);
            return list.Replace(newValue, predicate);
        }

        public async Task<IEnumerable<T>> CacheRemoveAsync<T>(string key, Func<T, bool> predicate)
        {
            var list = await CacheGetAsync<IEnumerable<T>>(key);
            return list.RemoveAll(predicate);
        }
    }

    static class EnumerableHelper
    {
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> source, T newValue, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var item in source)
            {
                yield return
                    predicate(item)
                        ? newValue
                        : item;
            }
        }

        public static IEnumerable<T> RemoveAll<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            foreach (var item in source)
            {
                if (predicate(item))
                    continue;
                yield return item;
            }
        }
    }
}
