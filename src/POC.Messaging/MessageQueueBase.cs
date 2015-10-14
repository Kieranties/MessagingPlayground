using System;
using System.Threading;
using System.Threading.Tasks;

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

        public virtual void Listen(Action<Message> onMessageReceived, CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(() => ListenInternal(onMessageReceived, cancellationToken), cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);            
        }

        protected virtual void ListenInternal(Action<Message> onMessageReceived, CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    break;
                }

                Receive(onMessageReceived, true);
                Thread.Sleep(100); //poll
            }
        }               

        public abstract void Receive(Action<Message> onMessageReceived, bool isAsync = false, int maxWaitMilliseconds = 0);

        public abstract void Dispose();
        
        public abstract IMessageQueue GetReplyQueue(Message message);

        public abstract IMessageQueue GetResponseQueue();               

        public abstract void Send(Message message);        
    }
}
