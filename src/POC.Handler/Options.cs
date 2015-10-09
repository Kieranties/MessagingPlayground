using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Handler
{
    public class Options
    {
        public string Queue { get; set; }

        public string MulticastAddress { get; set; }

        public string UnsubscribeHandler { get; set; }
    }
}
