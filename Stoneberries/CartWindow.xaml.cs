using Microsoft.AspNetCore.Cors.Infrastructure;
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
using System.IO;
using Stoneberries.Models;
using System.Globalization;

namespace Stoneberries
{
    /// <summary>
    /// Логика взаимодействия для CartWindow.xaml
    /// </summary>
    public partial class CartWindow : Window
    {
        public readonly ApplicationContext _applicationContext;
        private readonly CurrentUserService _currentUserService;
        private readonly CartService _cartService;
        public decimal cost = 0;
        public CartWindow(ApplicationContext applicationContext, CurrentUserService currentUserService, CartService cartService)
        {
            _applicationContext = applicationContext;
            _currentUserService = currentUserService;
            _cartService = cartService;
            InitializeComponent();
            DataContext = this;
            LoadCartItems();
        }
        private void SignUp_Click(object sender, MouseButtonEventArgs e)
        {
            var profileIcon = new ProfileIcon(_currentUserService, _applicationContext, _cartService);
            profileIcon.Click(this);
        }
        private void Main_Click(object sender, MouseButtonEventArgs e)
        {
            var mainWindow = new MainWindow(_applicationContext, _currentUserService, _cartService);
            mainWindow.Show();
            this.Hide();

        }
        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button.DataContext as CartItem;

            if (cartItem.Quantity > 1)
            {
                _cartService.UpdateCartItemQuantity(cartItem.Id, cartItem.Quantity - 1);
                LoadCartItems();
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button.DataContext as CartItem;

            _cartService.UpdateCartItemQuantity(cartItem.Id, cartItem.Quantity + 1);
            LoadCartItems();
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button.DataContext as CartItem;

            _cartService.RemoveFromCart(cartItem.Id);
            LoadCartItems();
        }

        private void LoadCartItems()
        {
            int count = 0;
            cost = 0;
            var cartItems = _cartService.GetCartItems(_currentUserService.LoggedInUser.Id);
            var cartItemsToLoad = cartItems.Select(cartItem => new CartItemToLoad
            {
                Id = cartItem.Id,
                CartId = cartItem.CartId,
                Cart = cartItem.Cart,
                ProductId = cartItem.ProductId,
                Product = new Product
                {
                    Id = cartItem.Product.Id,
                    Name = cartItem.Product.Name,
                    ImagePath = cartItem.Product.ImagePath,
                    Price = cartItem.Product.Price,
                },
                Quantity = cartItem.Quantity,
            }).ToList();
            foreach (var cartItem in cartItemsToLoad)
            {
                cartItem.Product.ImagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Products_Images_Croped", $"{cartItems.FirstOrDefault(c => c.Id == cartItem.Id).Product.ImagePath}.jpg");
                cost += cartItem.Product.Price*cartItem.Quantity;
                count += cartItem.Quantity;
            }
            cartItemsControl.ItemsSource = cartItemsToLoad;
            sum_cost.Content = cost.ToString("N2", CultureInfo.InvariantCulture) + "₽";
            if (count % 10 == 1 && count % 100 != 11)
            {
                sum_count.Content = $"{count} товар";
            }
            else if (count % 10 < 5)
            {
                sum_count.Content = $"{count} товара";
            }
            else
            {
                sum_count.Content = $"{count} товаров";
            }
            


        }

        private void Buy_Click(object sender, RoutedEventArgs e)
        {
            var user = _applicationContext.Users.FirstOrDefault(x => x.Id == _currentUserService.LoggedInUser.Id);
            if (user.Balance >= cost && cartItemsControl.Items.Count!=0)
            {
                NotifGrid.Visibility = Visibility.Visible;
                user.Balance = user.Balance - cost;
                _cartService.ClearCart(user.Id);
                LoadCartItems();
                NoMoney.Visibility = Visibility.Hidden;
            }
            else if(cartItemsControl.Items.Count != 0) 
            {
                NoMoney.Visibility = Visibility.Visible;
            }
        }
        private void HideNotifGrid(object sender, RoutedEventArgs e)
        {
            NotifGrid.Visibility = Visibility.Hidden;
        }
    }
}
