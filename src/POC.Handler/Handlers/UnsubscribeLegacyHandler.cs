using Microsoft.Framework.Logging;
using POC.Integration;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using POC.Messages.Event;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;

namespace POC.Handler.Handlers
{
    public class UnsubscribeLegacyHandler : MSMQMessageHandler<UserUnsubscribed>
    {
        private readonly ILogger<UnsubscribeLegacyHandler> _logger;

        public UnsubscribeLegacyHandler(ILogger<UnsubscribeLegacyHandler> logger)
        {
            _logger = logger;
        }        

        protected override void Handle(Message message, UserUnsubscribed content)
        {
            _logger.LogInformation($"[Received] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
            var workflow = new UnsubscribeLegacyWorkflow(content.EmailAddress);
            workflow.Run();
            _logger.LogInformation($"[Processed] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
        }
    }
}
