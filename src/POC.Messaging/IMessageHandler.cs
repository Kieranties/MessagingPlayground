using System;

namespace POC.Messaging
{
    public interface IMessageHandler
    {
        Type HandleType { get; }

        void Handle(Message message, IMessageQueue sourceQueue);        
    }
}
