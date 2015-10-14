using System;
using Microsoft.ServiceBus.Messaging;

namespace POC.Messaging.Azure
{
    public class AzureTopicClient : IAzureClient
    {
        private readonly TopicClient _client;

        public AzureTopicClient(AzureMessageQueueConnection connection)
        {
            _client = TopicClient.CreateFromConnectionString(connection.Endpoint, connection.Name);
        }

        public void Dispose()
        {
            
        }

        public BrokeredMessage Receive(int maxWaitMilliseconds = 0)
        {
            throw new NotImplementedException();
        }

        public void Send(BrokeredMessage message) => _client.Send(message);

        public void OnMessage(Action<BrokeredMessage> callback, OnMessageOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
