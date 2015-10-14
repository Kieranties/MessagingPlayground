using Microsoft.Framework.Logging;
using System.Collections.Generic;

namespace POC.Messaging.Azure
{
    public class AzureQueueFactory : MessageQueueFactoryBase<AzureMessageQueueConnection>
    {
        private readonly ILogger<AzureQueueFactory> _logger;

        public AzureQueueFactory(IList<AzureMessageQueueConnection> connectionMapping, ILogger<AzureQueueFactory> logger)
            : base(connectionMapping)
        {
            _logger = logger;
        }        
        
        public override IMessageQueue Connect(IMessageQueueConnection connection)
        {
            var azureConnection = (AzureMessageQueueConnection)connection;

            _logger.LogInformation($"[Registered] {connection.Name} | {connection.Direction} | {connection.Pattern} | {azureConnection.Endpoint}");
            return new AzureMessageQueue(azureConnection, this);
        }

        public override IMessageQueue Create(IMessageQueueConnection connection) => Connect(connection);
    }
}
