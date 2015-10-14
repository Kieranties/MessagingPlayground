using Microsoft.Framework.Logging;
using POC.Integration.Workflows;
using POC.Messages.Event;
using POC.Messaging;
using System;

namespace POC.Handler.Handlers
{
    public class UnsubscribeFulfilmentHandler : MessageHandlerBase<UserUnsubscribed>
    {
        private readonly ILogger<UnsubscribeFulfilmentHandler> _logger;

        public UnsubscribeFulfilmentHandler(ILogger<UnsubscribeFulfilmentHandler> logger)
        {
            _logger = logger;
        }

        public override void Handle(Message message, IMessageQueue sourceQueue)
        {
            var data = message.BodyAs<UserUnsubscribed>();

            _logger.LogInformation($"[{DateTime.Now}] Started: {data.EmailAddress}");

            var workflow = new UnsubscribeFulfilmentWorkflow(data.EmailAddress);
            workflow.Run();

            _logger.LogInformation($"[{DateTime.Now}] Finished: {data.EmailAddress}");
        }
    }
}
