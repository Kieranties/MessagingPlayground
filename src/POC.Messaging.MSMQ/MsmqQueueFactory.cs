using Microsoft.Framework.Logging;
using System.Collections.Generic;
using System.Messaging;

namespace POC.Messaging.MSMQ
{
    public class MsmqQueueFactory : MessageQueueFactoryBase<IMessageQueueConnection>
    {        
        private readonly ILogger<MsmqQueueFactory> _logger;
        
        public MsmqQueueFactory(IList<IMessageQueueConnection> connectionMapping, ILogger<MsmqQueueFactory> logger)
            : base(connectionMapping)
        {            
            _logger = logger;
        }

        public override IMessageQueue Connect(IMessageQueueConnection connection)
        {
            _logger.LogInformation($"[Registered] {connection.Name} | {connection.Direction} | {connection.Pattern} | {connection.Address}");
            return new MsmqMessageQueue(connection, this);
        }

        public override IMessageQueue Create(IMessageQueueConnection connection)
        {
            if (!MessageQueue.Exists(connection.Address))
            {
                MessageQueue.Create(connection.Address);
            }

            return Connect(connection);
        }
    }
}
