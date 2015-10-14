using System;
using System.Collections.Generic;

namespace POC.Messaging
{
    public abstract class MessageQueueBase<T> : IMessageQueue
    {
        public MessageQueueBase(string address, Direction direction, MessagePattern pattern, IMessageQueueFactory queueFactory, IDictionary<string, object> properties)
        {
            Address = address;
            Direction = direction;
            Pattern = pattern;            
            QueueFactory = queueFactory;
            Properties = Properties ?? new Dictionary<string, object>();
        }

        public string Address { get; protected set; }        

        public IDictionary<string, object> Properties { get; protected set; }

        protected MessagePattern Pattern { get; set; }

        protected Direction Direction { get; set; }

        protected IMessageQueueFactory QueueFactory { get; set; }

        protected T Queue { get; set; }

        public virtual void Listen(Action<Message> onMessageReceived)
        {
            while (true)
            {
                Receive(onMessageReceived);
            }
        }

        public abstract void Receive(Action<Message> onMessageReceved);

        public abstract void Dispose();
        
        public abstract IMessageQueue GetReplyQueue(Message message);

        public abstract IMessageQueue GetResponseQueue();               

        public abstract void Send(Message message);        
    }
}
