using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace POC.Integration.Workflows
{
    public class UnsubscribeLegacyWorkflow
    {
        public UnsubscribeLegacyWorkflow(string emailAddress)
        {
            EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }

        public void Run()
        {
            Thread.Sleep(UnsubscribeWorkflow.StepDuration);
        }
    }
}
