using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Messages.Event
{
    public class UserUnsubscribed
    {
        public string EmailAddress { get; set; }
    }
}
