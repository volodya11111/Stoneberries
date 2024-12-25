using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoneberries
{
    public class CurrentUserService
    {
        public User? LoggedInUser { get; private set; }
        public string Status { get; private set; }

        public void SetUser(User user)
        {
            LoggedInUser = user;
        }
    }
}