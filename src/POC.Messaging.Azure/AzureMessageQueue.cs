using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.IO;

namespace POC.Messaging.Azure
{
    public class AzureMessageQueue : MessageQueueBase<QueueClient>
    {        
        public AzureMessageQueue(string address, string name, Direction direction, MessagePattern pattern, IMessageQueueFactory queueFactory, IDictionary<string, object> properties)
            : base(address, direction, pattern, queueFactory, properties)
        {
            Name = name;
            Endpoint = address;
            
            // set address to correct format
            Address = $"{Name}||{Endpoint}";

            Queue = QueueClient.CreateFromConnectionString(address, name);
        }

        protected IMessageQueue RepsonseQueue { get; set; }

        protected string Endpoint { get; set; }

        protected string Name { get; set; }


        public override void Dispose()
        {
            Queue.Close();
            RepsonseQueue.Dispose();
        }

        public override IMessageQueue GetReplyQueue(Message message)
        {
            return QueueFactory.CreateOutbound(message.ResponseAddress, MessagePattern.RequestResponse);
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            if (RepsonseQueue != null)
                return RepsonseQueue;
                        
            var queueName = $"{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}-{Guid.NewGuid().ToString("D")}||{Endpoint}";

            RepsonseQueue = QueueFactory.CreateInbound(queueName, MessagePattern.RequestResponse, null);

            return RepsonseQueue;
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
            Queue.Send(brokeredMessage);
        }
    }
}
