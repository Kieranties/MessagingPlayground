using POC.Messages;
using POC.Messages.Event;
using System;
using System.Linq;
using System.Messaging;

namespace POC.Integration.Workflows
{
    public class UnsubscribeWorkflow
    {
        public static int StepDuration = 10;

        public UnsubscribeWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }

        public void Run()
        {
            PersistAsUnsubscribed();
            NotifyUserUnsubscribed();
        }

        private void NotifyUserUnsubscribed()
        {
            var @event = new UserUnsubscribed
            {
                EmailAddress = EmailAddress
            };

            using (var queue = new MessageQueue("FormatName:MULTICAST=234.1.1.1:8001"))
            {
                var message = new Message
                {
                    BodyStream = @event.ToJsonStream(),
                    Label = @event.GetMessageType(),
                    Recoverable = true
                };

                queue.Send(message);
            }
        }
        
        private void PersistAsUnsubscribed()
        {
            var user = Data.UserRepository.Instance.Users.FirstOrDefault(x => x.EmailAddress == EmailAddress);
            if(user != null)
            {
                user.IsUnsubscribed = true;
                user.UnsubscribedAt = DateTime.Now;
            }                
        }
    }
}
