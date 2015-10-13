using POC.Messages.Event;
using POC.Messaging;
using System;
using System.Linq;

namespace POC.Integration.Workflows
{
    public class UnsubscribeWorkflow
    {
        public static int StepDuration = 10;
        private readonly IMessageQueueFactory _queueFactory;

        public UnsubscribeWorkflow(string emailAddress, IMessageQueueFactory queueFactory)
        {
            EmailAddress = emailAddress;
            _queueFactory = queueFactory;
        }

        public string EmailAddress { get; }

        public void Run()
        {
            PersistAsUnsubscribed();
            NotifyUserUnsubscribed();
        }

        private void NotifyUserUnsubscribed()
        {
            var @event = new UserUnsubscribed { EmailAddress = EmailAddress };
            var message = new Message { Body = @event };
            var queue = _queueFactory.CreateOutbound("unsubscribed-event", MessagePattern.PublishSubscribe);
            queue.Send(message);
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
