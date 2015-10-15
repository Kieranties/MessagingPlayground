using Microsoft.Framework.Logging;
using System;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Configuration;
using POC.Messaging;
using Microsoft.Dnx.Runtime;
using System.Threading;

namespace POC.Handler
{
    public class Program
    {        
        public Program()
        {
            Console.CancelKeyPress += (a, e) => Environment.Exit(0);
        }

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

            logger.LogInformation("Ctrl + C to exit");

            while (true)
            {
                var cancelSource = Start(queue, handlerFactory, logger);
                Console.ReadKey(true);
                cancelSource.Cancel();
                logger.LogInformation("Press any key to start listening");
                Console.ReadKey(true);
            }            
        }
        
        private CancellationTokenSource Start(IMessageQueue queue, IMessageHandlerFactory handlerFactory, ILogger logger)
        {
            var cancelSource = new CancellationTokenSource();

            logger.LogInformation("Running");
            logger.LogInformation("Press any key to pause");

            queue.Listen(msg =>
            {
                var handler = handlerFactory.GetHandler(msg.Body.GetType());
                handler.Handle(msg, queue);
            }, cancelSource.Token);

            return cancelSource;
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
