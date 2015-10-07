using Microsoft.Framework.Logging;
using NetMQ;
using System.Diagnostics;

namespace ZMQ.Sockets
{
    public class Push : AbstractSocket
    {
        public Push(ILogger<Push> logger) : base(logger)
        {

        }

        public override void Start(Options options)
        {
            Logger.LogInformation($"Starting [{options.Id}]");

            using (var socket = Context.CreatePushSocket())
            {
                socket.Connect(options.Address());
                
                var sw = Stopwatch.StartNew();
                
                for (var i = 0; i < options.Messages; i++)
                {
                    // write a message
                    socket.SendFrame($"Message: [{options.Id}] - {i}");
                }

                Logger.LogInformation($" [{options.Id}] - Sent {options.Messages} messages in: {sw.ElapsedMilliseconds} milliseconds");
            }            
        }
    }
}
