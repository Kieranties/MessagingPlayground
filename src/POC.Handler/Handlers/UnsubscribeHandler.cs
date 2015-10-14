using Microsoft.Framework.Logging;
using POC.Integration.Workflows;
using POC.Messages.Commands;
using POC.Messaging;
using System;

namespace POC.Handler.Handlers
{
    public class UnsubscribeHandler : MessageHandlerBase<UnsubscribeCommand>
    {
        private readonly ILogger<UnsubscribeHandler> _logger;
        private readonly IMessageQueueFactory _queueFactory;

        public UnsubscribeHandler(ILogger<UnsubscribeHandler> logger, IMessageQueueFactory queueFactory)
        {
            _logger = logger;
            _queueFactory = queueFactory;
        }

        public override void Handle(Message message, IMessageQueue sourceQueue)
        {
            var data = message.BodyAs<UnsubscribeCommand>();

            _logger.LogInformation($"[{DateTime.Now}] Started: {data.EmailAddress}");                
                        
            var workflow = new UnsubscribeWorkflow(data.EmailAddress, _queueFactory);
            workflow.Run();

            _logger.LogInformation($"[{DateTime.Now}] Finished: {data.EmailAddress}");
        }
    }
}
