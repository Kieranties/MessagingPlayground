namespace POC.Messaging
{
    public interface IMessageQueueConnection
    {        
        string Name { get; }

        string Address { get; }

        MessagePattern Pattern { get; }

        Direction Direction { get; }        
    }
}
