using Microsoft.Framework.Logging;
using POC.Integration.Workflows;
using POC.Messages.Event;
using POC.Messaging;
using System;

namespace POC.Handler.Handlers
{
    public class UnsubscribeLegacyHandler : MessageHandlerBase<UserUnsubscribed>
    {
        private readonly ILogger<UnsubscribeLegacyHandler> _logger;

        public UnsubscribeLegacyHandler(ILogger<UnsubscribeLegacyHandler> logger)
        {
            _logger = logger;
        }

        public override void Handle(Message message, IMessageQueue sourceQueue)
        {
            var data = message.BodyAs<UserUnsubscribed>();

            _logger.LogInformation($"[{DateTime.Now}] Started: {data.EmailAddress}");

            var workflow = new UnsubscribeLegacyWorkflow(data.EmailAddress);
            workflow.Run();

            _logger.LogInformation($"[{DateTime.Now}] Finished: {data.EmailAddress}");
        }
    }
}
