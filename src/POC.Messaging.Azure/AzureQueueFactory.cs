using Microsoft.Framework.Logging;
using Microsoft.ServiceBus;
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

            _logger.LogInformation($"[Registered] {connection.Id} | {connection.Direction} | {connection.Pattern} | {azureConnection.Endpoint}");
            return new AzureMessageQueue(azureConnection, this);
        }

        public override IMessageQueue Create(IMessageQueueConnection connection)
        {
            var azureConnection = (AzureMessageQueueConnection)connection;

            var manager = NamespaceManager.CreateFromConnectionString(azureConnection.Endpoint);

            if (string.IsNullOrWhiteSpace(azureConnection.Subscription))
            {
                if (!manager.QueueExists(azureConnection.Name))
                {
                    manager.CreateQueue(azureConnection.Name);
                }
            }
            else
            {
                if (!manager.TopicExists(azureConnection.Name))
                {
                    manager.CreateTopic(azureConnection.Name);
                }

                if (!manager.SubscriptionExists(azureConnection.Name, azureConnection.Subscription))
                {
                    manager.CreateSubscription(azureConnection.Name, azureConnection.Subscription);
                }
            }

            return Connect(connection);
        }
    }
}
