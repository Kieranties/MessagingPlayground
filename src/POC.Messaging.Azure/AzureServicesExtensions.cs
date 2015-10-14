using Microsoft.Framework.Configuration;
using POC.Messaging;
using POC.Messaging.Azure;
using System.Collections.Generic;

namespace Microsoft.Framework.DependencyInjection
{
    public static class AzureServicesExtensions
    {
        public static IServiceCollection AddAzure(this IServiceCollection services, IConfigurationSection queueConfig)
        {
            var options = new Options();
            queueConfig.Bind(options);

            return services.AddSingleton<IMessageQueueFactory>(sp => ActivatorUtilities.CreateInstance<AzureQueueFactory>(sp, options.Queues));
        }

        private class Options
        {
            public List<AzureMessageQueueConnection> Queues { get; set; } = new List<AzureMessageQueueConnection>();
        }
    }
}
