using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ThrottleService.Resources;

namespace ThrottleService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThrottleController : ControllerBase
    {
        // GET: api/Throttle
        [HttpGet]
        [Throttle]
        public ActionResult Get(Log log)
        {
            //Do something ...
            return Ok();
        }

    }

    public class MySampleActionFilter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Do something before the action executes.
            //MyDebug.Write(MethodBase.GetCurrentMethod(), context.HttpContext.Request.Path);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Do something after the action executes.
            //MyDebug.Write(MethodBase.GetCurrentMethod(), context.HttpContext.Request.Path);
        }
    }

    public class ThrottleAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            object log;
            context.ActionArguments.TryGetValue("log", out log);
            if (log != null)
            {
                context.Result = new BadRequestObjectResult(context.ModelState);
            }

            // Do various logic to determine if we should respond with a 429
        }
    }

}
