using Microsoft.Framework.Logging;
using Microsoft.ServiceBus;
using System.Collections.Generic;

namespace POC.Messaging.Azure
{
    public class AzureQueueFactory : MessageQueueFactoryBase
    {
        private readonly ILogger<AzureQueueFactory> _logger;

        public AzureQueueFactory(ILogger<AzureQueueFactory> logger, IDictionary<string, string> addressMapping)
            : base(addressMapping)
        {
            _logger = logger;
        }

        protected override IMessageQueue CreateQueue(string name, Direction direction, MessagePattern pattern, IDictionary<string, object> properties)
        {
            var address = GetAddress(name);

            if (string.IsNullOrEmpty(address))
            {
                var splitName = name.Split(new[] { "||" }, System.StringSplitOptions.RemoveEmptyEntries);
                name = splitName[0];
                address = splitName[1];
                
                var namespaceManager = NamespaceManager.CreateFromConnectionString(address);
                if (!namespaceManager.QueueExists(name))
                {
                    namespaceManager.CreateQueue(name);
                }
            }
            else
            {
                var splitAddress = address.Split(new[] { "||" }, System.StringSplitOptions.RemoveEmptyEntries);
                name = splitAddress[0];
                address = splitAddress[1];
            }            

            _logger.LogInformation($"[Registered] {direction} Queue: {name} | {pattern} | {address}");
            return new AzureMessageQueue(address, name, direction, pattern, this, properties);
        }
    }
}
