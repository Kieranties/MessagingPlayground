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

        public BrokeredMessage Receive(int maxWaitMilliseconds = 0)
        {
            if (maxWaitMilliseconds != 0)
            {
                return _client.Receive(TimeSpan.FromMilliseconds(maxWaitMilliseconds));
            }
            else
            {
                return _client.Receive();
            }
        }

        public void OnMessage(Action<BrokeredMessage> callback, OnMessageOptions options) => _client.OnMessage(callback, options);

        public void Dispose()
        {

        }        
    }
}
