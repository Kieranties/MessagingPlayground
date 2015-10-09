using Microsoft.Framework.Logging;
using System;
using System.Messaging;
using POC.Messages.Commands;
using Microsoft.Framework.DependencyInjection;
using POC.Messages;
using POC.Messages.Queries;
using Microsoft.Framework.Configuration;
using POC.Handler.Handlers;
using POC.Integration;
using POC.Messages.Event;

namespace POC.Handler
{
    public class Program
    {
        private ILogger<Program> _logger;
        private IServiceProvider _services;
        
        public void Main(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            var options = new Options();
            config.Bind(options);

            _services = BuildServiceProvider(options);
            _logger = _services.GetRequiredService<ILogger<Program>>();

            
            using (var queue = new MessageQueue(options.Queue))
            {
                _logger.LogInformation($"Listening on {options.Queue}");
                if (!string.IsNullOrWhiteSpace(options.MulticastAddress))
                {
                    queue.MulticastAddress = options.MulticastAddress;
                    _logger.LogInformation($"Multicast address: {options.MulticastAddress}");
                }

                while (true)
                {                    
                    var message = queue.Receive();

                    // make generic type - this can be improved upon
                    var handlerType = typeof(MSMQMessageHandler<>).MakeGenericType(Type.GetType(message.Label));                    
                    var handler = _services.GetService(handlerType) as MSMQMessageHandler;
                    handler.Handle(message);                    
                }
            }
        }
        
        private IServiceProvider BuildServiceProvider(Options options)
        {
            var services = new ServiceCollection().AddLogging();

            services.AddTransient<MSMQMessageHandler<DoesUserExistRequest>, DoesUserExistRequestHandler>();
            services.AddTransient<MSMQMessageHandler<UnsubscribeCommand>, UnsubscribeHandler>();

            if (!string.IsNullOrWhiteSpace(options.UnsubscribeHandler))
            {
                services.AddTransient(typeof(MSMQMessageHandler<UserUnsubscribed>), Type.GetType(options.UnsubscribeHandler));                
            }

            var provider = services.BuildServiceProvider();

            Configure(provider);

            return provider;
        }

        private void Configure(IServiceProvider services)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            loggerFactory.MinimumLevel = LogLevel.Debug;
            loggerFactory.AddConsole(loggerFactory.MinimumLevel);
        }
    }
}
