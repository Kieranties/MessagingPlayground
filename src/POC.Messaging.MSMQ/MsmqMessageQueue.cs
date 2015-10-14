using System;
using System.Messaging;
using System.Threading.Tasks;
using MsmqMessage = System.Messaging.Message;

namespace POC.Messaging.MSMQ
{
    public class MsmqMessageQueue : MessageQueueBase<MessageQueue>
    {   
        public MsmqMessageQueue(IMessageQueueConnection connection,IMessageQueueFactory queueFactory)
            : base(connection, queueFactory)
        {
            Queue = new MessageQueue(connection.Address);
        }
        
        protected IMessageQueue ResponseQueue { get; set; }
        
        public override IMessageQueue GetReplyQueue(Message message)
        {
            if (!(Connection.Pattern == MessagePattern.RequestResponse && Connection.Direction == Direction.Inbound))
                throw new InvalidOperationException("Cannot get a reply queue except for inbound request-response");

            return QueueFactory.Create(message.ResponseConnection);
        }

        public override IMessageQueue GetResponseQueue()
        {
            if (!(Connection.Pattern == MessagePattern.RequestResponse && Connection.Direction == Direction.Outbound))
                throw new InvalidOperationException("Cannot get a response queue except for outbound request-response");

            if (ResponseQueue != null)
                return ResponseQueue;
                        
            // make unique based on timestamp and guid
            var address = $".\\private$\\{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}:{Guid.NewGuid().ToString().Trim('{','}')}";
            var connection = new MessageQueueConnection { Address = address, Pattern = MessagePattern.RequestResponse, Direction = Direction.Inbound };
            ResponseQueue = QueueFactory.Create(connection);
            return ResponseQueue;
        }

        public override void Receive(Action<Message> onMessageReceived, bool isAsync = false, int maxWaitMilliseconds = 0)
        {
            MsmqMessage inbound;
            if (maxWaitMilliseconds > 0)
            {
                inbound = Queue.Receive(TimeSpan.FromMilliseconds(maxWaitMilliseconds));
            }
            else
            {
                inbound = Queue.Receive();
            }

            var message = Message.FromJson(inbound.BodyStream);
            if (isAsync)
            {
                Task.Run(() => onMessageReceived(message));
            }
            else
            {
                onMessageReceived(message);
            }
        }

        public override void Send(Message message)
        {
            var outbound = new MsmqMessage { BodyStream = message.ToJsonStream() };
            if (message.ResponseConnection != null)
            {
                outbound.ResponseQueue = new MessageQueue(message.ResponseConnection.Address);
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
