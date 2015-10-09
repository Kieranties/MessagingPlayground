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
    public class UnsubscribeFulfilmentHandler : MSMQMessageHandler<UserUnsubscribed>
    {
        private readonly ILogger<UnsubscribeFulfilmentHandler> _logger;

        public UnsubscribeFulfilmentHandler(ILogger<UnsubscribeFulfilmentHandler> logger)
        {
            _logger = logger;
        }        

        protected override void Handle(Message message, UserUnsubscribed content)
        {
            _logger.LogInformation($"[Received] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
            var workflow = new UnsubscribeFulfilmentWorkflow(content.EmailAddress);
            workflow.Run();
            _logger.LogInformation($"[Processed] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
        }
    }
}
