using NetMQ;
using Microsoft.Framework.Logging;

namespace ZMQ.Sockets
{
    public class Pull : AbstractSocket
    {
        public Pull(ILogger<Pull> logger) : base(logger)
        {
            
        }

        public override void Start(Options options)
        {
            Logger.LogInformation($"Starting [{options.Id}]");

            using (var socket = Context.CreatePullSocket())
            {
                socket.Bind(options.Address());

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
