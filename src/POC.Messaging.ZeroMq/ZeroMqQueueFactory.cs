using Microsoft.Framework.Logging;
using NetMQ;
using System.Collections.Generic;

namespace POC.Messaging.ZeroMq
{
    public class ZeroMqQueueFactory : MessageQueueFactoryBase
    {        
        private readonly ILogger<ZeroMqQueueFactory> _logger;
        private readonly NetMQContext _context = NetMQContext.Create();
        
        public ZeroMqQueueFactory(IDictionary<string, string> addressMapping, ILogger<ZeroMqQueueFactory> logger)
            : base(addressMapping)
        {            
            _logger = logger;
        }        

        protected override IMessageQueue CreateQueue(string name, Direction direction, MessagePattern pattern, IDictionary<string, object> properties)
        {
            var address = GetAddress(name) ?? name;

            _logger.LogInformation($"[Registered] {direction} Queue: {name} | {pattern} | {address}");
            return new ZeroMqMessageQueue(_context, address, direction, pattern, properties);
        }
    }
}
