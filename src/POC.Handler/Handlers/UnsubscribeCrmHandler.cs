using Microsoft.Framework.Logging;
using POC.Integration;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using POC.Messages.Event;
using System;
using System.Messaging;

namespace POC.Handler.Handlers
{
    public class UnsubscribeCrmHandler : MSMQMessageHandler<UserUnsubscribed>
    {
        private readonly ILogger<UnsubscribeCrmHandler> _logger;

        public UnsubscribeCrmHandler(ILogger<UnsubscribeCrmHandler> logger)
        {
            _logger = logger;
        }        

        protected override void Handle(Message message, UserUnsubscribed content)
        {
            _logger.LogInformation($"[Received] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
            var workflow = new UnsubscribeCrmWorkflow(content.EmailAddress);
            workflow.Run();
            _logger.LogInformation($"[Processed] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
        }
    }
}
