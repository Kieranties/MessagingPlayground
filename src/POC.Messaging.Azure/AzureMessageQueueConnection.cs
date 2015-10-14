namespace POC.Messaging.Azure
{
    public class AzureMessageQueueConnection : IMessageQueueConnection
    {
        public string Address => $"{Name}||{Endpoint}";

        public Direction Direction { get; set; }

        public MessagePattern Pattern { get; set; }

        public string Name { get; set; }

        public string Endpoint { get; set; }
    }
}
