using Microsoft.Framework.Logging;
using System;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Configuration;
using POC.Messaging;
using POC.Messaging.MSMQ;
using POC.Messaging.ZeroMq;
using POC.Messaging.Azure;

namespace POC.Handler
{
    public class Program
    {
        public void Main(string[] args)
        {
            var config = new ConfigurationBuilder(".")
                .AddCommandLine(args)
                .AddJsonFile("azure.queues.json")
                .Build();

            var options = new Options();
            config.Bind(options);

            var services = BuildServiceProvider(options);
            var logger = services.GetRequiredService<ILogger<Program>>();
            var handlerFactory = services.GetRequiredService<IMessageHandlerFactory>();
            var queueFactory = services.GetRequiredService<IMessageQueueFactory>();
                        
            var queue = queueFactory.CreateInbound(options.ListenTo, options.Pattern);
            queue.Listen(msg =>
            {
                var handler = handlerFactory.GetHandler(msg.Body.GetType());
                handler.Handle(msg, queue);
            });            
        }
        
        // Composition rootgithub
        private IServiceProvider BuildServiceProvider(Options options)
        {
            var services = new ServiceCollection().AddLogging();

            services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();
            services.AddSingleton<IMessageQueueFactory>(sp => ActivatorUtilities.CreateInstance<AzureQueueFactory>(sp, options.Queues));
            
            if (!string.IsNullOrWhiteSpace(options.Handler))
            {
                services.AddTransient(typeof(IMessageHandler), Type.GetType(options.Handler));
            }

            var provider = services.BuildServiceProvider();

            // configure
            var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(loggerFactory.MinimumLevel);

            return provider;
        }
    }
}
