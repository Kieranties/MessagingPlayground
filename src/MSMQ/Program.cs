using Microsoft.Dnx.Runtime.Common.CommandLine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;

namespace MSMQ
{
    public class Program
    {

        private const string HELP_TEMPLATE = "-?|-h|--help";

        private static Action<MessageQueue, string> simpleSend = (q, m) => q.Send(m);
        private static Action<MessageQueue, string> transactionSend = (q, m) =>
        {
            var tx = new MessageQueueTransaction();
            tx.Begin();
            q.Send(m, tx);
            tx.Commit();
        };

        public int Main(string[] args)
        {

            var app = new CommandLineApplication
            {
                Name = "dnx msmq",
                FullName = "MSMQ Utils"
            };

            app.HelpOption(HELP_TEMPLATE);

            app.OnExecute(() => ShowHelp(app));

            app.Command("create", create =>
                {
                    create.Description = "Create a queue";
                    create.HelpOption(HELP_TEMPLATE);

                    var path = create.Argument("[path]", "The path for the queue");
                    var transactional = create.Option("-t|--transactional", "Make the queue transactional", CommandOptionType.NoValue);
                    var secure = create.Option("-s|--secure", "Require authentication", CommandOptionType.NoValue);
                    var encryption = create.Option("-e|--encryption <encryption>", "Set encyption type", CommandOptionType.SingleValue);

                    create.OnExecute(() =>
                    {
                        var queue = MessageQueue.Create(path.Value, transactional.HasValue());
                        queue.Authenticate = secure.HasValue();

                        var enc = EncryptionRequired.Optional;
                        Enum.TryParse(encryption.Value(), out enc);
                        queue.EncryptionRequired = enc;

                        return 0;
                    });
                }
            );

            app.Command("send", send =>
            {
                send.Description = "Send a message to a queue";
                send.HelpOption(HELP_TEMPLATE);

                var path = send.Argument("[path]", "The path for the queue");
                var message = send.Argument("[message]", "The message to send");
                var recoverable = send.Option("-r|--recoverable", "Make the message recoverable", CommandOptionType.NoValue);
                var count = send.Option("-c|--count <count>", "The number of messages to send", CommandOptionType.SingleValue);
                var transaction = send.Option("-t|--transaction", "Use a transaction to send each message", CommandOptionType.NoValue);

                send.OnExecute(() =>
                {
                    using (var queue = new MessageQueue(path.Value))
                    {
                        // set all message recoverable flag
                        // if in a transaction message is durable by default
                        queue.DefaultPropertiesToSend.Recoverable = recoverable.HasValue();

                        var handler = transaction.HasValue() ? transactionSend : simpleSend;

                        var sw = Stopwatch.StartNew();

                        for (int x = 0; x < (count.HasValue() ? int.Parse(count.Value()) : 1000); x++)
                        {
                            handler(queue, $"[{x}] - {message.Value}");
                        }

                        Console.WriteLine(sw.ElapsedMilliseconds);
                    }
                    
                    return 0;
                });
            });

            app.Command("purge", purge =>
            {
                purge.Description = "Purge all messages from a queue";
                purge.HelpOption(HELP_TEMPLATE);

                var path = purge.Argument("[path]", "The path for the queue");

                purge.OnExecute(() =>
                {
                    using(var queue = new MessageQueue(path.Value))
                    {
                        queue.Purge();
                    }

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
