using Microsoft.Framework.Logging;
using POC.Integration;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Threading.Tasks;

namespace POC.Handler.Handlers
{
    public class UnsubscribeHandler : MSMQMessageHandler<UnsubscribeCommand>
    {
        private readonly ILogger<UnsubscribeHandler> _logger;

        public UnsubscribeHandler(ILogger<UnsubscribeHandler> logger)
        {
            _logger = logger;
        }        

        protected override void Handle(Message message, UnsubscribeCommand content)
        {
            _logger.LogInformation($"[Received] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
            var workflow = new UnsubscribeWorkflow(content.EmailAddress);
            workflow.Run();
            _logger.LogInformation($"[Processed] unsubscribe for {content.EmailAddress}, at: {DateTime.Now}");
        }
    }
}
