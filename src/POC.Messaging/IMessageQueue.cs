using System;
using System.Threading;

namespace POC.Messaging
{
    public interface IMessageQueue : IDisposable
    {
        IMessageQueueConnection Connection { get; }
                
        void Send(Message message);

        void Listen(Action<Message> onMessageReceived, CancellationToken cancellationToken);

        void Receive(Action<Message> onMessageReceived, bool isAsync = false, int maxWaitMilliseconds = 0);
        
        IMessageQueue GetResponseQueue();

        IMessageQueue GetReplyQueue(Message message);
    }
}
