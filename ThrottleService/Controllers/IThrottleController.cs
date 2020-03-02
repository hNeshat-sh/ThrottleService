using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThrottleService.Controllers
{
    public interface IThrottleController
    {
        IDistributedCache Cache { get; }
    }
}
