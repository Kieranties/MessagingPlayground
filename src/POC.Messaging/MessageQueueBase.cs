using System;
using System.Collections.Generic;

namespace POC.Messaging
{
    public abstract class MessageQueueBase : IMessageQueue
    {
        public MessageQueueBase(string address, Direction direction, MessagePattern pattern, IDictionary<string, object> properties)
        {
            Direction = direction;
            Pattern = pattern;
            Address = address;
            Properties = Properties ?? new Dictionary<string, object>();
        }

        public string Address { get; protected set; }        

        public IDictionary<string, object> Properties { get; protected set; }

        protected MessagePattern Pattern { get; set; }

        protected Direction Direction { get; set; }

        public abstract void Dispose();
        
        public abstract IMessageQueue GetReplyQueue(Message message);

        public abstract IMessageQueue GetResponseQueue();        

        public abstract void Listen(Action<Message> onMessageReceived);

        public abstract void Receive(Action<Message> onMessageReceved);

        public abstract void Send(Message message);        
    }
}
