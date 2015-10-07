using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace POC.Integration.Workflows
{
    public class UnsubscribeWorkflow
    {
        private const int StepDuration = 10000;

        public UnsubscribeWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }

        public void Run()
        {
            PersistAsUnsubscribed();
            UnsubscribeInLegacySystem();
            SetCrmMailingReference();
            CancelPendingMailshots();
        }

        private void CancelPendingMailshots()
        {
            Thread.Sleep(StepDuration);
        }

        private void SetCrmMailingReference()
        {
            Thread.Sleep(StepDuration);
        }

        private void UnsubscribeInLegacySystem()
        {
            Thread.Sleep(StepDuration);
        }

        private void PersistAsUnsubscribed()
        {
            Thread.Sleep(StepDuration);
        }
    }
}
