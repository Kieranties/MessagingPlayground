using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Messaging
{
    public class MessageQueueConnection : IMessageQueueConnection
    {
        public string Name { get; set; }

        public string Address { get; set; }

        public Direction Direction { get; set; }

        public MessagePattern Pattern { get; set; }
    }
}
