using Microsoft.Framework.Logging;
using System.Collections.Generic;
using System.Messaging;

namespace POC.Messaging.MSMQ
{
    public class MsmqQueueFactory : MessageQueueFactoryBase
    {        
        private readonly ILogger<MsmqQueueFactory> _logger;
        
        public MsmqQueueFactory(IDictionary<string, string> addressMapping, ILogger<MsmqQueueFactory> logger)
            :base(addressMapping)
        {            
            _logger = logger;
        }        

        protected override IMessageQueue CreateQueue(string name, Direction direction, MessagePattern pattern, IDictionary<string, object> properties)
        {
            var address = GetAddress(name);

            if (string.IsNullOrEmpty(address))
            {
                MessageQueue.Create(name); // treat name as an address
                address = name;
            }

            _logger.LogInformation($"[Registered] {direction} Queue: {name} | {pattern} | {address}");
            return new MsmqMessageQueue(address, direction, pattern, this, properties);
        }
    }
}
