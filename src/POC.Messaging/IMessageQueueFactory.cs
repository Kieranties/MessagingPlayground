namespace POC.Messaging
{
    public interface IMessageQueueFactory
    {
        IMessageQueue Get(string id);

        IMessageQueue Connect(IMessageQueueConnection connection);

        IMessageQueue Create(IMessageQueueConnection connection);        
    }
}
