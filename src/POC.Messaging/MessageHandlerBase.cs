using System;

namespace POC.Messaging
{
    public abstract class MessageHandlerBase<T> : IMessageHandler
    {
        public virtual Type HandleType => typeof(T);

        public abstract void Handle(Message message, IMessageQueue sourceQueue);
    }
}
