namespace POC.Messaging.Azure
{
    public class AzureMessageQueueConnection : IMessageQueueConnection
    {
        public string Id => string.IsNullOrWhiteSpace(Subscription) ? Name : $"{Name}:{Subscription}";

        public string Address => $"{Id}||{Endpoint}";

        public Direction Direction { get; set; }

        public MessagePattern Pattern { get; set; }

        public string Name { get; set; }

        public string Subscription { get; set; }

        public string Endpoint { get; set; }
    }
}
