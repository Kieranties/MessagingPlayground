using POC.Integration.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Integration.Queries
{
    public class DoesUserExist
    {
        public static bool Execute(string emailAddress) => UserRepository.Instance.Users.Any(x => x.EmailAddress == emailAddress);
    }
}
