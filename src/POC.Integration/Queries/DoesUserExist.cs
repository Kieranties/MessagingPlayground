using POC.Integration.Data;
using System.Linq;

namespace POC.Integration.Queries
{
    public class DoesUserExist
    {        
        public static bool Execute(string emailAddress) => UserRepository.Instance.Users.Any(x => x.EmailAddress == emailAddress);
    }
}
