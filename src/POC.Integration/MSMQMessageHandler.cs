using POC.Messages;
using System.Messaging;

namespace POC.Integration
{
    public abstract class MSMQMessageHandler : IMessageHandler<Message>
    {
        public abstract void Handle(Message message);
    }

    public abstract class MSMQMessageHandler<T> : MSMQMessageHandler
    {
        public override void Handle(Message message) => Handle(message, message.BodyStream.FromJson<T>());
        protected abstract void Handle(Message sourceMessage, T content);
    }
}
