using NetMQ;
using Microsoft.Framework.Logging;

namespace ZMQ.Sockets
{
    public class Response : AbstractSocket
    {
        public Response(ILogger<Response> logger) : base(logger)
        {
            
        }

        public override void Start(Options options)
        {
            Logger.LogInformation($"Starting [{options.Id}]");

            using (var socket = Context.CreateResponseSocket())
            {
                socket.Bind(options.Address());

                while (true)
                {
                    // read a message
                    var message = socket.ReceiveFrameString();
                    Logger.LogDebug($"[{options.Id}] Received - {message}");                

                    // reply with a response
                    socket.SendFrame($"Thanks for {message} from {options.Id}");
                }
            }            
        }
    }
}
