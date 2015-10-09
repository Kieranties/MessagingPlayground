using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Integration.Data
{
    public class User
    {
        public int Id { get; set; }

        public string EmailAddress { get; set; }

        public bool IsUnsubscribed { get; set; } = false;

        public DateTime UnsubscribedAt { get; set; } = DateTime.MinValue;
    }
}
