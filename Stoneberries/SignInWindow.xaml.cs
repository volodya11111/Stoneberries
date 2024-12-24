using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Stoneberries
{
    public partial class SignInWindow : Window
    {
        public readonly ApplicationContext _applicationContext;
        private readonly CurrentUserService _currentUserService;
        private Window _lastWindow;
        public SignInWindow(ApplicationContext applicationContext, CurrentUserService currentUserService, Window lastWindow)
        {
            InitializeComponent();
            _applicationContext = applicationContext;
            _currentUserService = currentUserService;
            _lastWindow = lastWindow;
        }
        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var signInAccount = new SignInAccount();
            var user = signInAccount.SignIn(loginTextBox.Text, passwordBox.Password, _applicationContext);
            if (user != null)
            {
                _currentUserService.SetUser(user);
                _lastWindow.Show();
                this.Hide();
            }
            else
            {
                wrongLogPass.Visibility = Visibility.Visible;
            }
        }
        private void registerTextBlock(object sender, MouseButtonEventArgs e)
        {
            SignIn.Visibility = Visibility.Hidden;
            SignUp.Visibility = Visibility.Visible;
        }
        private void RegButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User();
            user.Login = loginTextBox_reg.Text;
            user.Password = passwordTextBox_reg.Text;
            user.Name = nameTextBox_reg.Text;
            empty_login.Visibility = Visibility.Hidden;
            empty_name.Visibility = Visibility.Hidden;
            empty_pass.Visibility = Visibility.Hidden;
            busy_login.Visibility = Visibility.Hidden;
            if (_applicationContext.Users.FirstOrDefault(x => x.Login == user.Login) == null)
            {
                if (user.Login == "")
                {
                    empty_login.Visibility = Visibility.Visible;
                }
                else if (user.Password == "")
                {
                    empty_pass.Visibility = Visibility.Visible;
                }
                else if (user.Name == "")
                {
                    empty_name.Visibility = Visibility.Visible;
                }
                else
                {
                    _applicationContext.Users.Add(user);
                    _applicationContext.SaveChanges();
                    _lastWindow.Show();
                    this.Close();
                }
            }
            else
            {
                busy_login.Visibility= Visibility.Visible;
                
            }
        }

        private void Back_click(object sender, MouseButtonEventArgs e)
        {
            SignIn.Visibility = Visibility.Visible;
            SignUp.Visibility = Visibility.Hidden;
        }

        private void BackToLast_click(object sender, MouseButtonEventArgs e)
        {
            _lastWindow.Show();
            this.Close();
        }
    }
}
