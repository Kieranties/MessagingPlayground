using System;
using System.Collections.Generic;
using System.Messaging;
using MsmqMessage = System.Messaging.Message;

namespace POC.Messaging.MSMQ
{
    public class MsmqMessageQueue : MessageQueueBase
    {   
        public MsmqMessageQueue(string address, Direction direction, MessagePattern pattern, IDictionary<string, object> properties = null)
            : this(new MessageQueue(address), direction, pattern, properties)
        {
            
        }

        private MsmqMessageQueue(MessageQueue queue, Direction direction, MessagePattern pattern, IDictionary<string, object> properties = null)
            : base(queue.Path, direction, pattern, properties)
        {
            Queue = queue;
        }

        protected MessageQueue Queue { get; set; }

        protected IMessageQueue ResponseQueue { get; set; }


        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Inbound))
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request-response");

            return new MsmqMessageQueue(message.ResponseAddress, Direction.Outbound, MessagePattern.RequestResponse, null);
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Pattern == MessagePattern.RequestResponse && Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            if (ResponseQueue != null)
                return ResponseQueue;
                        
            // make unique based on timestamp and guid
            var address = $".\\private$\\{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}:{Guid.NewGuid().ToString().Trim('{','}')}";
            ResponseQueue = new MsmqMessageQueue(MessageQueue.Create(address), Direction.Inbound, MessagePattern.RequestResponse, null);
            return ResponseQueue;
        }
       
        public override void Listen(Action<Message> onMessageReceived)
        {
            while (true)
            {
                Receive(onMessageReceived);
            }
        }

        public override void Receive(Action<Message> onMessageReceived)
        {
            var inbound = Queue.Receive();
            var message = Message.FromJson(inbound.BodyStream);
            onMessageReceived(message);
        }

        public override void Send(Message message)
        {
            var outbound = new MsmqMessage { BodyStream = message.ToJsonStream() };
            if (!string.IsNullOrWhiteSpace(message.ResponseAddress))
            {
                outbound.ResponseQueue = new MessageQueue(message.ResponseAddress);
            }

            Queue.Send(outbound);
        }

        public override void Dispose()
        {
            Queue.Dispose();
            if (ResponseQueue != null)
                ResponseQueue.Dispose();
        }
    }
}
