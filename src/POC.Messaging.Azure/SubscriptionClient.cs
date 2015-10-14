using System;
using Microsoft.ServiceBus.Messaging;

namespace POC.Messaging.Azure
{
    public class AzureSubscriptionClient : IAzureClient
    {
        private readonly SubscriptionClient _client;

        public AzureSubscriptionClient(AzureMessageQueueConnection connection)
        {
            _client = SubscriptionClient.CreateFromConnectionString(connection.Endpoint, connection.Name, connection.Subscription);
        }

        public void Send(BrokeredMessage message)
        {
            throw new NotImplementedException();
        }

        public BrokeredMessage Receive() => _client.Receive();

        public void Dispose()
        {

        }        
    }
}
