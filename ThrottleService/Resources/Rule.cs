using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThrottleService.Resources
{
    public class Rule
    {
        public int Id { get; set; }
        public string Pattern { get; set; }
        public LimitType Limit { get; set; }
    }

    public enum LimitType : int
    {
        TenPerSecond = 1,
        FiftyPerMinute = 2,
        HundredPerHours = 3
    }
}
