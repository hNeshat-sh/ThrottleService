using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ThrottleService.Resources;

namespace ThrottleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThrottleController : ControllerBase, IThrottleController
    {
        public readonly IDistributedCache _cache;
        IDistributedCache IThrottleController.Cache => _cache;
        public ThrottleController(IDistributedCache distributedCache)
        {
            _cache = distributedCache;
        }

        // GET: api/Throttle
        [HttpGet]
        [Throttle]
        public ActionResult Get(Log log)
        {
            //Do something ...
            return Ok();
        }
    }
}
