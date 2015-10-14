namespace POC.Messaging
{
    public interface IMessageQueueFactory
    {
        IMessageQueue Get(string name);

        IMessageQueue Connect(IMessageQueueConnection connection);

        IMessageQueue Create(IMessageQueueConnection connection);        
    }
}
