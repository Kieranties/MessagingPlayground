using System;

namespace POC.Messaging
{
    public interface IMessageQueue : IDisposable
    {
        IMessageQueueConnection Connection { get; }
                
        void Send(Message message);

        void Listen(Action<Message> onMessageReceived);

        void Receive(Action<Message> onMessageReceived);        

        IMessageQueue GetResponseQueue();

        IMessageQueue GetReplyQueue(Message message);
    }
}
