using System;

namespace POC.Messaging
{
    public abstract class MessageQueueBase<T> : IMessageQueue
    {
        public MessageQueueBase(IMessageQueueConnection connection, IMessageQueueFactory queueFactory)
        {
            Connection = connection;  
            QueueFactory = queueFactory;
        }

        public IMessageQueueConnection Connection { get; }

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
