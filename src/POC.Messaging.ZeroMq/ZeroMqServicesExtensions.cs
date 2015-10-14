using Microsoft.Framework.Configuration;
using POC.Messaging;
using POC.Messaging.ZeroMq;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Framework.DependencyInjection
{
    public static class ZeroMqServicesExtensions
    {
        public static IServiceCollection AddZeroMq(this IServiceCollection services, IConfigurationSection queueConfig)
        {
            var options = new Options();            
            queueConfig.Bind(options);

            return services.AddSingleton<IMessageQueueFactory>(sp => ActivatorUtilities.CreateInstance<ZeroMqQueueFactory>(sp, options.Queues.OfType<IMessageQueueConnection>().ToList()));
        }

        private class Options
        {
            public List<MessageQueueConnection> Queues { get; set; } = new List<MessageQueueConnection>();
        }
    }
}
