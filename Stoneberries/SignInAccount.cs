using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stoneberries
{
    public class SignInAccount
    {
        public User SignIn(string login, string password, ApplicationContext db)
        {
            var user = db.Users.FirstOrDefault(x => x.Login == login);
            if (user == null)
            {
                return null;
            }
            else if (user.Password == password)
            {
                return user;
            }
            return null;
        }
    }
}
