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
using ThrottleService.Helpers;

namespace ThrottleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RuleController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        public RuleController(IDistributedCache distributedCache)
        {
            _cache = distributedCache;
        }

        // GET: api/Rule
        [HttpGet]
        public IEnumerable<Rule> Get()
        {
            var rules = _cache.CacheGet<IEnumerable<Rule>>("Rules");
            return rules.ToArray();
        }

        //GET: api/Rule/5
        [HttpGet("{id}", Name = "Get")]
        public async Task<ActionResult<Rule>> Get(int id)
        {
            var rule = await _cache.CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (rule == null)
                return NotFound();
            return rule;
        }

        // POST: api/Rule
        [HttpPost]
        public async Task<ActionResult<Rule>> Post(Rule rule)
        {
            var rules = (await _cache.CacheGetAsync<IEnumerable<Rule>>("Rules")).ToList();
            rules.Add(rule);
            await _cache.CacheSaveAsync("Rules", rules.ToArray());
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
            var _rule = await _cache.CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (_rule == null)
                return NotFound();
            var rules = await _cache.CacheReplaceAsync<Rule>("Rules", a => a.Id == id, rule);
            await _cache.CacheSaveAsync("Rules", rules.ToArray());
            return NoContent();
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Rule>> Delete(int id)
        {
            var rule = await _cache.CacheFindAsync<Rule>("Rules", a => a.Id == id);
            if (rule == null)
            {
                return NotFound();
            }
            var rules = await _cache.CacheRemoveAsync<Rule>("Rules", a => a.Id == id);
            await _cache.CacheSaveAsync("Rules", rules.ToArray());
            return rule;
        }


    }

}
