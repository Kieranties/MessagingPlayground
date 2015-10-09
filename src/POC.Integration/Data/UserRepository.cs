using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POC.Integration.Data
{
    public class UserRepository
    {
        private static UserRepository _instance = new UserRepository();

        private readonly List<User> _users;

        protected UserRepository()
        {
            _users = new List<User>
            {
                new User { Id = 1, EmailAddress = "111@example.com" },
                new User { Id = 2, EmailAddress = "222@example.com" },
                new User { Id = 3, EmailAddress = "333@example.com" }
            };
        }

        public static UserRepository Instance => _instance;

        public IEnumerable<User> Users => _users;
    }
}
