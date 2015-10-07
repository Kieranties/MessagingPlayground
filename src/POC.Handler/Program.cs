using Microsoft.Framework.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using POC.Messages.Commands;
using POC.Integration.Workflows;
using Microsoft.Framework.DependencyInjection;

namespace POC.Handler
{
    public class Program
    {
        private readonly ILogger<Program> _logger;
        private readonly IServiceProvider _services;

        public Program()
        {
            _services = BuildServiceProvider();
            _logger = _services.GetRequiredService<ILogger<Program>>();
        }

        public void Main(string[] args)
        {
            using(var queue = new MessageQueue(".\\private$\\poc.messagequeue.unsubscribe-tx"))
            {
                while (true)
                {
                    _logger.LogInformation("Listening");
                    using(var transaction = new MessageQueueTransaction())
                    {
                        transaction.Begin(); // locks the mssage in the queue
                        var message = queue.Receive();
                        var bodyReader = new StreamReader(message.BodyStream);
                        var jsonBody = bodyReader.ReadToEnd();
                        var unsubscribeMessage = JsonConvert.DeserializeObject<UnsubscribeCommand>(jsonBody);
                        var workflow = new UnsubscribeWorkflow(unsubscribeMessage.EmailAddress);
                        _logger.LogDebug($"Starting unsubscribe for {unsubscribeMessage.EmailAddress} ");
                        workflow.Run();
                        _logger.LogDebug($"Unsubscribe complete for {unsubscribeMessage.EmailAddress} ");
                        transaction.Commit(); // removes the mssage from the queue as is completed
                    }                    
                }
            }
        }
        
        private IServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection().AddLogging();
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
