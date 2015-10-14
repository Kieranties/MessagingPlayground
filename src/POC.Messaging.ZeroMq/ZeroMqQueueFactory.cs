using Microsoft.Framework.Logging;
using NetMQ;
using System.Collections.Generic;

namespace POC.Messaging.ZeroMq
{
    public class ZeroMqQueueFactory : MessageQueueFactoryBase<IMessageQueueConnection>
    {        
        private readonly ILogger<ZeroMqQueueFactory> _logger;
        private readonly NetMQContext _context = NetMQContext.Create();
        
        public ZeroMqQueueFactory(IList<IMessageQueueConnection> connectionMapping, ILogger<ZeroMqQueueFactory> logger)
            : base(connectionMapping)
        {            
            _logger = logger;
        }        

        public override IMessageQueue Connect(IMessageQueueConnection connection)
        {
            _logger.LogInformation($"[Registered] {connection.Id} | {connection.Direction} | {connection.Pattern} | {connection.Address}");
            return new ZeroMqMessageQueue(_context, connection, this);
        }

        public override IMessageQueue Create(IMessageQueueConnection connection) => Connect(connection);

    }
}
