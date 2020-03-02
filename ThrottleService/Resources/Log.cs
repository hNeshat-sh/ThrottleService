using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ThrottleService.Resources
{
    public class Log
    {
        public string Url { get; set; }
        public string IP { get; set; }
        public DateTime DateTimeReceived { get; set; }
    }
}
