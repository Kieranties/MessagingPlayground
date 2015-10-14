using System;
using Microsoft.ServiceBus.Messaging;

namespace POC.Messaging.Azure
{
    public class AzureQueueClient : IAzureClient
    {
        private readonly QueueClient _client;

        public AzureQueueClient(AzureMessageQueueConnection connection)
        {
            _client = QueueClient.CreateFromConnectionString(connection.Endpoint, connection.Name);
        }

        public void Dispose()
        {
            
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

        public void Send(BrokeredMessage message) => _client.Send(message);

        public void OnMessage(Action<BrokeredMessage> callback, OnMessageOptions options) => _client.OnMessage(callback, options);
    }
}
