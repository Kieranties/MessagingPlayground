using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Messages.Queries
{
    public class DoesUserExistRequest
    {
        public string EmailAddress { get; set; }
    }

    public class DoesUserExistResponse
    {
        public bool Exists { get; set; }
    }
}
