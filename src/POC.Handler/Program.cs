using Microsoft.Framework.Logging;
using System;
using System.Messaging;
using POC.Messages.Commands;
using POC.Integration.Workflows;
using Microsoft.Framework.DependencyInjection;
using POC.Messages;
using POC.Messages.Queries;
using POC.Integration.Queries;
using Microsoft.Framework.Configuration;

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
            var config = new ConfigurationBuilder().AddCommandLine(args).Build();
            var options = new Options();
            config.Bind(options);

            using(var queue = new MessageQueue(options.Queue))
            {
                while (true)
                {
                    _logger.LogInformation($"Listening on {options.Queue}");
                    var message = queue.Receive();
                    var messageBody = message.BodyStream.FromJson(message.Label);
                    var messageType = messageBody.GetType();
                    if (messageType == typeof(UnsubscribeCommand))
                    {
                        Unsubscribe((UnsubscribeCommand)messageBody);
                    }
                    else if (messageType == typeof(DoesUserExistRequest))
                    {
                        CheckUserExists((DoesUserExistRequest)messageBody, message);
                    }
                }
            }
        }

        /// <summary>
        /// Send a message to check if a user exists using a Request-Response pattern
        /// </summary>
        /// <param name="message"></param>
        /// <param name="rawMessage"></param>
        private void CheckUserExists(DoesUserExistRequest message, Message rawMessage)
        {
            _logger.LogInformation($"Starting CheckUserExists for {message.EmailAddress}, at: {DateTime.Now}");

            var userExists = new DoesUserExistResponse
            {
                Exists = DoesUserExist.Execute(message.EmailAddress)
            };

            using(var queue = rawMessage.ResponseQueue)
            {
                var responseMessage = new Message
                {
                    BodyStream = userExists.ToJsonStream(),
                    Label = userExists.GetMessageType()
                };

                queue.Send(responseMessage);
            }

            _logger.LogInformation($"Returned {userExists.Exists} for CheckUserExists for {message.EmailAddress}, at: {DateTime.Now}");
        }

        private void Unsubscribe(UnsubscribeCommand message)
        {
            _logger.LogInformation($"Starting unsubscribe for {message.EmailAddress}, at: {DateTime.Now}");
            var workflow = new UnsubscribeWorkflow(message.EmailAddress);
            workflow.Run();
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
