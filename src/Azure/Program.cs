using Microsoft.Dnx.Runtime.Common.CommandLine;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Azure
{
    public class Program
    {
        private const string HELP_TEMPLATE = "-?|-h|--help";

        public int Main(string[] args)
        {
            var app = new CommandLineApplication
            {
                Name = "dnx Azure",
                FullName = "Azure Message Queue Utils"
            };

            app.HelpOption(HELP_TEMPLATE);

            app.OnExecute(() => ShowHelp(app));

            app.Command("send", send =>
            {
                send.Description = "Send a message";
                send.HelpOption(HELP_TEMPLATE);

                var queueName = send.Argument("[queueName]", "The name of the queue");
                var message = send.Argument("[message]", "The message to send");
                var connectionString = send.Argument("[connectionString]", "The connection string");

                send.OnExecute(() =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(connectionString.Value);
                    var queue = factory.CreateQueueClient(queueName.Value);
                    var packet = new BrokeredMessage(message.Value);

                    queue.Send(packet);

                    return 0;
                });
            });

            app.Command("receive", receive =>
            {
                receive.Description = "Receive a message";
                receive.HelpOption(HELP_TEMPLATE);

                var queueName = receive.Argument("[queueName]", "The name of the queue");
                var connectionString = receive.Argument("[connectionString]", "The connection string");

                receive.OnExecute(() =>
                {
                    var factory = MessagingFactory.CreateFromConnectionString(connectionString.Value);
                    var queue = factory.CreateQueueClient(queueName.Value);
                    var message = queue.Receive();

                    Console.WriteLine(message.GetBody<string>());

                    message.Complete(); // must signal to queue that the message has been handled

                    return 0;
                });
            });

            return app.Execute(args);
        }

        private int ShowHelp(CommandLineApplication app)
        {
            app.ShowHelp();
            return 0;
        }
    }
}
