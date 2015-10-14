using Microsoft.Framework.Logging;
using System;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Configuration;
using POC.Messaging;

namespace POC.Handler
{
    public class Program
    {
        public void Main(string[] args)
        {
            var runtimeConfig = new ConfigurationBuilder()
                .AddCommandLine(args)                
                .Build();

            var options = new Options();
            runtimeConfig.Bind(options);

            var queueConfig = new ConfigurationBuilder(".")
                .AddJsonFile("queues.json")
                .Build()
                .GetSection(options.QueueType);
                        
            var services = BuildServiceProvider(options, queueConfig);
            var logger = services.GetRequiredService<ILogger<Program>>();
            var handlerFactory = services.GetRequiredService<IMessageHandlerFactory>();
            var queueFactory = services.GetRequiredService<IMessageQueueFactory>();
                        
            var queue = queueFactory.Get(options.ListenTo);
            queue.Listen(msg =>
            {
                var handler = handlerFactory.GetHandler(msg.Body.GetType());
                handler.Handle(msg, queue);
            });            
        }
        
        // Composition root
        private IServiceProvider BuildServiceProvider(Options options, IConfigurationSection queueConfig)
        {
            var services = new ServiceCollection().AddLogging();

            services.AddSingleton<IMessageHandlerFactory, MessageHandlerFactory>();

            switch (options.QueueType)
            {
                case "zeromq":
                    services.AddZeroMq(queueConfig);
                    break;
                case "msmq":
                    services.AddMsmq(queueConfig);
                    break;
                case "azure":
                    services.AddAzure(queueConfig);
                    break;
                default:
                    throw new Exception($"Could not resolve queue type {options.QueueType}");
            }           
            
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
