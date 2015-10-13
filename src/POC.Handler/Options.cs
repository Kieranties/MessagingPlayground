using POC.Messaging;
using System.Collections.Generic;

namespace POC.Handler
{
    public class Options
    {
        public string ListenTo { get; set; }

        public MessagePattern Pattern { get; set; }

        public string Handler { get; set; }

        public Dictionary<string, string> Queues { get; set; } = new Dictionary<string, string>();
    }
}
