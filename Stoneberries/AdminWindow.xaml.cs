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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.EntityFrameworkCore;

namespace Stoneberries
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private ObservableCollection<Product> _products;
        private Window _lastWindow;
        private ApplicationContext _applicationContext;
        private string _baseDir;
        private string _selectedImagePath;
        public AdminWindow(Window lastWindow, ApplicationContext applicationContext)
        {
            InitializeComponent();
            _lastWindow = lastWindow;
            _applicationContext = applicationContext;
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _baseDir = baseDir;
            LoadProducts();
        }
        private void LoadProducts()
        {
            var productsFromDb = _applicationContext.Products;

            string imagePath = Path.Combine(_baseDir, "Products_images");
            string outputPath = Path.Combine(_baseDir, "Products_Images_Croped");
            int width = 175;
            int height = 200;

            ImageHelper.CropImageAndSave(imagePath, outputPath, width, height);
            _products = new ObservableCollection<Product>();
            foreach (var product in productsFromDb)
            {
                _products.Add(new Product
                {
                    Id = product.Id,
                    Name = product.Name,
                    Price = product.Price,
                    ImagePath = Path.Combine(outputPath, $"{product.ImagePath}.jpg")
                });
            }
            ProductsListBox.ItemsSource = _products;
        }

        private void Back(object sender, MouseButtonEventArgs e)
        {
            _lastWindow.Show();
            this.Close();
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var item = button.DataContext as Product;
            var productToDelete = _applicationContext.Products.FirstOrDefault(p => p.Id == item.Id);
            _applicationContext.Products.Remove(productToDelete);
            _applicationContext.SaveChanges();
            LoadProducts();
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProductGrid.Visibility = Visibility.Visible;
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "Image files (*.jpg;)|*.jpg;",
                Title = "Выберите изображение"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedImagePath = openFileDialog.FileName;
                SelectedImagePreview.Source = new BitmapImage(new Uri(_selectedImagePath));
            }
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
                if (value > 9999)
                {
                    NumericTextBox.Text = "9999";
                    NumericTextBox.SelectionStart = NumericTextBox.Text.Length;
                }
            }
        }
        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            string name = NameTextBox.Text;
            decimal price = Convert.ToDecimal(NumericTextBox.Text);
            string destinationFolder = "Products_Images";

            if (string.IsNullOrEmpty(_selectedImagePath))
            {
                MessageBox.Show("Выберите изображение.");
                return;
            }
            string fileName = Guid.NewGuid().ToString();
            string destinationPath = Path.Combine(destinationFolder, $"{fileName}.jpg");

            File.Copy(_selectedImagePath, destinationPath, true);

            var newProduct = new Product
            {
                Name = name,
                Price = price,
                ImagePath = fileName
            };
            _applicationContext.Products.Add(newProduct);
            _applicationContext.SaveChanges();

            LoadProducts();
        }

        private void HideAddProductGrid(object sender, MouseButtonEventArgs e)
        {
            AddProductGrid.Visibility = Visibility.Hidden;
        }
    }
}
