using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;

namespace POC.Messaging.Azure
{
    public class AzureMessageQueue : MessageQueueBase<QueueClient>
    {
        private readonly TopicClient _topicClient;

        public AzureMessageQueue(AzureMessageQueueConnection connection, IMessageQueueFactory queueFactory)
            : base(connection, queueFactory)
        {
            AzureConnection = connection;
            switch (connection.Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    _topicClient = TopicClient.CreateFromConnectionString(connection.Endpoint, connection.Name);
                    break;
                default:
                    Queue = QueueClient.CreateFromConnectionString(connection.Endpoint, connection.Name);
                    break;
            }            
        }

        protected AzureMessageQueueConnection AzureConnection { get; set; }

        protected IMessageQueue ResponseQueue { get; set; }
                
        public override void Dispose()
        {
            Queue.Close();
            ResponseQueue.Dispose();
        }

        public override IMessageQueue GetReplyQueue(Message message) => QueueFactory.Create(message.ResponseConnection);

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Connection.Pattern == MessagePattern.RequestResponse && Connection.Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            if (ResponseQueue != null)
                return ResponseQueue;
                        
            var queueName = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString("D")}||{AzureConnection.Endpoint}";
            var connection = new AzureMessageQueueConnection { Name = queueName, Endpoint = AzureConnection.Endpoint, Direction = Direction.Inbound, Pattern = MessagePattern.RequestResponse };
            ResponseQueue = QueueFactory.Create(connection);

            return ResponseQueue;
        }

        public override void Receive(Action<Message> onMessageReceved)
        {
            var packet = Queue.Receive();
            var messageStream = packet.GetBody<Stream>();
            var message = Message.FromJson(messageStream);
            onMessageReceved(message);

            packet.Complete(); // azure needs to signal the message has been handled
        }

        public override void Send(Message message)
        {
            var brokeredMessage = new BrokeredMessage(message.ToJsonStream(), true);

            switch (Connection.Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    _topicClient.Send(brokeredMessage);
                    break;
                default:                    
                    Queue.Send(brokeredMessage);
                    break;
            }
        }
    }
}
