using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Stoneberries
{
    public class ProfileIcon
    {
        CurrentUserService _currentUserService;
        ApplicationContext _applicationContext;
        CartService _cartService;
        public ProfileIcon(CurrentUserService currentUserService, ApplicationContext applicationContext, CartService cartService)
        {
            _currentUserService = currentUserService;
            _applicationContext = applicationContext;
            _cartService = cartService;
        }
        public void Click(Window window)
        {
            if (_currentUserService.LoggedInUser != null)
            {
                var profileWindow = new Profile(_currentUserService, _applicationContext, _cartService);
                profileWindow.Show();
                window.Hide();
            }
            else
            {
                var signInWindow = new SignInWindow(_applicationContext, _currentUserService, window);
                signInWindow.Show();
                window.Hide();
            }
        }
    }
}
