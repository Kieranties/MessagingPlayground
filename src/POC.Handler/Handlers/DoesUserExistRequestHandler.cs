using POC.Messages.Queries;
using POC.Integration;
using System;
using Microsoft.Framework.Logging;
using POC.Integration.Queries;
using System.Messaging;
using POC.Messages;

namespace POC.Handler.Handlers
{
    public class DoesUserExistRequestHandler : MSMQMessageHandler<DoesUserExistRequest>
    {
        private readonly ILogger<DoesUserExistRequestHandler> _logger;

        public DoesUserExistRequestHandler(ILogger<DoesUserExistRequestHandler> logger)
        {
            _logger = logger;            
        }
        
        protected override void Handle(Message message, DoesUserExistRequest content)
        {
            _logger.LogInformation($"[Recevied] CheckUserExists for {content.EmailAddress}, at: {DateTime.Now}");

            var userExists = new DoesUserExistResponse
            {
                Exists = DoesUserExist.Execute(content.EmailAddress)
            };

            using (var queue = message.ResponseQueue)
            {
                var responseMessage = new Message
                {
                    BodyStream = userExists.ToJsonStream(),
                    Label = userExists.GetMessageType()
                };

                queue.Send(responseMessage);
            }

            _logger.LogInformation($"[Processed] {userExists.Exists} for CheckUserExists for {content.EmailAddress}, at: {DateTime.Now}");
        }
    }
}
