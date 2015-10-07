using Microsoft.Framework.Logging;
using NetMQ;
using System.Diagnostics;
using System.Threading;

namespace ZMQ.Sockets
{
    public class Publisher : AbstractSocket
    {
        public Publisher(ILogger<Publisher> logger) : base(logger)
        {

        }

        public override void Start(Options options)
        {
            Logger.LogInformation($"Starting [{options.Id}]");

            using (var socket = Context.CreatePublisherSocket())
            {
                socket.Bind(options.Address());
                
                var sw = Stopwatch.StartNew();

                var i = 0;
                while(true)
                {
                    // write some messages                    
                    socket.SendFrame($"Message A: [{options.Id}] - {i}");
                    socket.SendFrame($"Message B: [{options.Id}] - {i}");
                    i++;
                    Thread.Sleep(100);
                }
            }            
        }
    }
}
