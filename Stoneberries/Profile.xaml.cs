using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace Stoneberries
{
    /// <summary>
    /// Логика взаимодействия для Profile.xaml
    /// </summary>
    public partial class Profile : Window
    {
        CurrentUserService _currentUserService;
        ApplicationContext _applicationContext;
        private readonly CartService _cartService;
        private readonly ObservableCollection<Product> _products;
        public Profile(CurrentUserService currentUserService, ApplicationContext applicationContext, CartService cartService)
        {
            InitializeComponent();
            _currentUserService = currentUserService;
            var user = _currentUserService.LoggedInUser;
            login_label.Content = user.Login;
            name_textBox.Text = user.Name;
            balance_label.Content = user.Balance;
            _applicationContext = applicationContext;
            _cartService = cartService;
        }
        private void Main_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow(_applicationContext, _currentUserService, _cartService);
            mainWindow.Show();
            this.Hide();
        }
        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUserService.LoggedInUser == null)
            {
                var loginWindow = new SignInWindow(_applicationContext, _currentUserService, this);
                this.Hide();
                loginWindow.Show();
                return;
            }          
            var cartWindow = new CartWindow(_applicationContext, _currentUserService, _cartService);
            cartWindow.Show();
            this.Hide();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            _applicationContext.Users.FirstOrDefault(x => x.Id == _currentUserService.LoggedInUser.Id).Name = name_textBox.Text;
            _applicationContext.SaveChanges();
        }

        private void AddMoney_Click(object sender, RoutedEventArgs e)
        {
            AddBalanceGrid.Visibility = Visibility.Visible;
        }
        private void TopUp_Click(object sender, RoutedEventArgs e)
        {
            var newBalance = Convert.ToDecimal(NumericTextBox.Text);
            var user = _applicationContext.Users.FirstOrDefault(x => x.Id == _currentUserService.LoggedInUser.Id);
            user.Balance = user.Balance + Convert.ToDecimal(NumericTextBox.Text);
            balance_label.Content = _applicationContext.Users.FirstOrDefault(x => x.Id == _currentUserService.LoggedInUser.Id).Balance;
            AddBalanceGrid.Visibility = Visibility.Hidden;
        }
        private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {           
            e.Handled = !int.TryParse(e.Text, out _);
        }
        private void NumericTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Back || e.Key == Key.Delete)
            {
                e.Handled = false;
            }
        }
        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(NumericTextBox.Text, out int value))
            {
                if (value > 99999)
                {
                    NumericTextBox.Text = "99999";
                    NumericTextBox.SelectionStart = NumericTextBox.Text.Length; 
                }
            }
        }

        private void HideAddBalanceGrid(object sender, MouseButtonEventArgs e)
        {
            AddBalanceGrid.Visibility = Visibility.Hidden;
        }

        private void ExitAccount_Click(object sender, MouseButtonEventArgs e)
        {
            _currentUserService.SetUser(null);
            Main_Click(sender, e);
        }
    }
}
