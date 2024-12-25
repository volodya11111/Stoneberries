using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.EntityFrameworkCore;

namespace Stoneberries
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly ApplicationContext _applicationContext;
        private readonly CurrentUserService _currentUserService;
        private readonly CartService _cartService;
        private readonly string _baseDir;
        public ObservableCollection<Product> Products { get; set; }
        public MainWindow(ApplicationContext applicationContext, CurrentUserService currentUserService, CartService cartService)
        {
            _applicationContext = applicationContext;
            _currentUserService = currentUserService;
            _cartService = cartService;

            InitializeComponent();

            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _baseDir = baseDir;

            Products = new ObservableCollection<Product>();
            LoadProducts();

            DataContext = this;

        }

        private void SignUp_Click(object sender, MouseButtonEventArgs e)
        {
            var profileIcon = new ProfileIcon(_currentUserService, _applicationContext, _cartService);
            profileIcon.Click(this);
            this.Hide();

        }

        private void LoadProducts(string query = "")
        {
            var productsFromDb = _applicationContext.Products
                .ToList()
                .Where(p => string.IsNullOrEmpty(query) || p.Name.IndexOf(query, StringComparison.OrdinalIgnoreCase) >= 0)
                .ToList();

            string imagePath = Path.Combine(_baseDir, "Products_images");
            string outputPath = Path.Combine(_baseDir, "Products_Images_Croped");
            int width = 175;
            int height = 200;

            ImageHelper.CropImageAndSave(imagePath, outputPath, width, height);

            Products.Clear();
            foreach (var product in productsFromDb)
            {
                Products.Add(new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImagePath = Path.Combine(outputPath, $"{product.ImagePath}.jpg")
                });
            }
        }

        private void AddToCart(object sender, RoutedEventArgs e)
        {
            if (_currentUserService.LoggedInUser != null)
            {
                var button = sender as Button;
                var product = button.DataContext as Product;
                _cartService.AddProductToCartAsync(product);
                if (notification.Visibility == Visibility.Hidden)
                {
                    Show_Notification();
                }
            }
            else
            {
                var signInWindow = new SignInWindow(_applicationContext, _currentUserService, this);
                signInWindow.Show();
                this.Hide();
            }
        }
        private async Task Show_Notification()
        {
            notification.Visibility = Visibility.Visible;
            await Task.Delay(3000);
            notification.Visibility = Visibility.Hidden;
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

        private void Search_Click(object sender, MouseButtonEventArgs e)
        {
            var query = SearchTextBox.Text;
            LoadProducts(query);
        }
    }
}
