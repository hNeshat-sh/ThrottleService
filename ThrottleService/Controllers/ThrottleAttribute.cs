using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using ThrottleService.Resources;
using ThrottleService.Helpers;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ThrottleService.Controllers
{
    public class ThrottleAttribute : ActionFilterAttribute
    {
        public override async void OnActionExecuting(ActionExecutingContext context)
        {
            object log;
            context.ActionArguments.TryGetValue("log", out log);
            if (log != null)
            {
                var _log = log as Log;
                _log.DateTimeReceived = DateTime.Now;
                var controller = ((IThrottleController)context.Controller);
                var cache = controller.Cache;
                var logs = (await cache.CacheGetListAsync<Log>("Logs")).ToList();
                var _lastReceive = logs.OrderBy(l => l.DateTimeReceived).LastOrDefault(l => l.Url == _log.Url);
                logs.Add(_log);
                await cache.CacheSaveAsync("Logs", logs.ToArray());
                await cache.RefreshAsync("Rules");
                var rules = cache.CacheGetList<Rule>("Rules");
                var _rules = rules.ToArray();
                var _matchRule = rules.FirstOrDefault(r => MatchUrl(_log.Url, r.Pattern));
                if (_matchRule == null ||
                    (_lastReceive != null &&
                    ((_matchRule.Limit == LimitType.TenPerSecond && (_log.DateTimeReceived - _lastReceive.DateTimeReceived).TotalMilliseconds < (1000 / 10))) ||
                     (_matchRule.Limit == LimitType.FiftyPerMinute && (_log.DateTimeReceived - _lastReceive.DateTimeReceived).TotalMilliseconds < (60000 / 50))) ||
                      (_matchRule.Limit == LimitType.HundredPerHours && (_log.DateTimeReceived - _lastReceive.DateTimeReceived).TotalSeconds < (3600 / 100)))
                {
                    // Do various logic to determine if we should respond with a 429
                    context.Result = new BadRequestObjectResult(context.ModelState);
                }
            }
        }

        private bool MatchUrl(string url, string pattern)
        {
            return url == pattern || url.StartsWith(pattern);
        }
    }
}
