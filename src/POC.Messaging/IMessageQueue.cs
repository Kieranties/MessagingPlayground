using System;
using System.Collections.Generic;

namespace POC.Messaging
{
    public interface IMessageQueue : IDisposable
    {
        string Address { get; }

        IDictionary<string, object> Properties { get; }
        
        void Send(Message message);

        void Listen(Action<Message> onMessageReceived);

        void Receive(Action<Message> onMessageReceived);        

        IMessageQueue GetResponseQueue();

        IMessageQueue GetReplyQueue(Message message);
    }
}
