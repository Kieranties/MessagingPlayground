using NetMQ;
using Microsoft.Framework.Logging;

namespace ZMQ.Sockets
{
    public class Subscriber : AbstractSocket
    {
        public Subscriber(ILogger<Subscriber> logger) : base(logger)
        {
            
        }

        public override void Start(Options options)
        {
            Logger.LogInformation($"Starting [{options.Id}]");

            using (var socket = Context.CreateSubscriberSocket())
            {
                socket.Connect(options.Address());
                socket.Subscribe(options.Topic); // if empty subscribes to all

                while (true)
                {
                    // read a message
                    var message = socket.ReceiveFrameString();
                    Logger.LogDebug($"[{options.Id}] Received - {message}");    
                }
            }            
        }
    }
}
