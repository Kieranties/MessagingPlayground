using Microsoft.ServiceBus.Messaging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace POC.Messaging.Azure
{
    public class AzureMessageQueue : MessageQueueBase<IAzureClient>
    {
        public AzureMessageQueue(AzureMessageQueueConnection connection, IMessageQueueFactory queueFactory)
            : base(connection, queueFactory)
        {
            AzureConnection = connection;

            switch (connection.Pattern)
            {
                case MessagePattern.PublishSubscribe:
                    if(connection.Direction == Direction.Inbound)
                    {
                        Queue = new AzureSubscriptionClient(connection);
                    }
                    else
                    {
                        Queue = new AzureTopicClient(connection);
                    }
                    break;
                default:
                    Queue = new AzureQueueClient(connection);
                    break;
            }            
        }

        protected AzureMessageQueueConnection AzureConnection { get; set; }

        protected IMessageQueue ResponseQueue { get; set; }
                
        public override void Dispose()
        {
            Queue.Dispose();
            ResponseQueue.Dispose();
        }

        public override IMessageQueue GetReplyQueue(Message message) => QueueFactory.Create(message.ResponseConnection);

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Connection.Pattern == MessagePattern.RequestResponse && Connection.Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            if (ResponseQueue != null)
                return ResponseQueue;
                        
            var queueName = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString("D")}";
            var connection = new AzureMessageQueueConnection { Name = queueName, Endpoint = AzureConnection.Endpoint, Direction = Direction.Inbound, Pattern = MessagePattern.RequestResponse };
            ResponseQueue = QueueFactory.Create(connection);

            return ResponseQueue;
        }

        protected override void ListenInternal(Action<Message> onMessageReceived, CancellationToken cancellationToken)
        {
            var options = new OnMessageOptions { MaxConcurrentCalls = 10 };

            Queue.OnMessage(message => HandleReceive(message, onMessageReceived), options);

            cancellationToken.WaitHandle.WaitOne();
        }

        public override void Receive(Action<Message> onMessageReceived, bool isAsync = false, int maxWaitMilliseconds = 0)
        {
            
            var packet = Queue.Receive(maxWaitMilliseconds);
            
            if (isAsync)
            {
                Task.Run(() => HandleReceive(packet, onMessageReceived));
            }
            else
            {
                HandleReceive(packet, onMessageReceived);
            }
        }

        protected virtual void HandleReceive(BrokeredMessage packet, Action<Message> onMessageReceived)
        {
            var messageStream = packet.GetBody<Stream>();
            var message = Message.FromJson(messageStream);
            onMessageReceived(message);

            packet.Complete(); // azure needs to signal the message has been handled
        }

        public override void Send(Message message)
        {
            var brokeredMessage = new BrokeredMessage(message.ToJsonStream(), true);
            Queue.Send(brokeredMessage);
        }        
    }
}
