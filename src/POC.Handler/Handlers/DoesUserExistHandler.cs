using Microsoft.Framework.Logging;
using POC.Integration.Queries;
using POC.Messages.Queries;
using POC.Messaging;
using System;

namespace POC.Handler.Handlers
{
    public class DoesUserExistHandler : MessageHandlerBase<DoesUserExistRequest>
    {
        private readonly ILogger<DoesUserExistHandler> _logger;

        public DoesUserExistHandler(ILogger<DoesUserExistHandler> logger)
        {
            _logger = logger;
        }

        public override void Handle(Message message, IMessageQueue sourceQueue)
        {
            var data = message.BodyAs<DoesUserExistRequest>();

            _logger.LogInformation($"[{DateTime.Now}] Started: {data.EmailAddress}");

            var userExists = new DoesUserExistResponse { Exists = DoesUserExist.Execute(data.EmailAddress) };
            var response = new Message { Body = userExists };

            var queue = sourceQueue.GetReplyQueue(message);
            queue.Send(response);

            _logger.LogDebug($"[{DateTime.Now}] User: {data.EmailAddress} Exists: {userExists.Exists}");
            _logger.LogInformation($"[{DateTime.Now}] Finished: {data.EmailAddress}");
        }
    }
}
