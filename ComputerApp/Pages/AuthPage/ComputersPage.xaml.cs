using ComputerApp.Data;
using System;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace ComputerApp.Pages.AuthPage
{
    /// <summary>
    /// Логика взаимодействия для ComputersPage.xaml
    /// </summary>
    public partial class ComputersPage : Page
    {
        private ComputerStoreEntities _db = ComputerStoreEntities.GetContext();
        public ObservableCollection<Computer> Computers { get; set; }

        public ComputersPage()
        {
            InitializeComponent();
            _db.Computers.Load();
            _db.ShoppingCarts.Load();
            _db.Buyers.Load();

            LoadComputersFromDatabase();
        }

        private void LoadComputersFromDatabase()
        {
            var userCartComputers = _db.ShoppingCarts
                                        .Where(cart => cart.BuyerID == CurrentUser.LoggedInUser.BuyerID)
                                        .Select(cart => cart.ComputersID)
                                        .ToList();

            Computers = new ObservableCollection<Computer>(_db.Computers.ToList());

            // Устанавливаем состояние IsInCart для каждого компьютера
            foreach (var computer in Computers)
            {
                computer.IsInCart = userCartComputers.Contains(computer.ComputersID);
            }

            DataContext = this;
        }

        private void AddComputerToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var computer = button?.DataContext as Computer;

            if (computer != null)
            {
                var userCart = _db.ShoppingCarts
                                  .FirstOrDefault(cart => cart.BuyerID == CurrentUser.LoggedInUser.BuyerID && cart.ComputersID == computer.ComputersID);

                if (userCart != null)
                {
                    // Компьютер уже в корзине, удаляем его
                    _db.ShoppingCarts.Remove(userCart);
                    computer.IsInCart = false;
                }
                else
                {
                    // Компьютера нет в корзине, добавляем его
                    userCart = new ShoppingCart
                    {
                        BuyerID = CurrentUser.LoggedInUser.BuyerID,
                        ComputersID = computer.ComputersID
                    };

                    _db.ShoppingCarts.Add(userCart);
                    computer.IsInCart = true;
                }

                _db.SaveChanges();
                // Обновляем коллекцию компьютеров
                ReloadPage();
            }
        }

        private void ReloadPage()
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FramePage.Navigate(new ComputersPage());
            }
        }

        private void LogoutAccClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                CurrentUser.Logout();
                mainWindow.FramePage.Navigate(new AuthPage());
                mainWindow.ComputersListBtn.Visibility = Visibility.Collapsed;
                mainWindow.ComponentListBtn.Visibility = Visibility.Collapsed;
                mainWindow.Height = 450;
            }
        }

        private void CartClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FramePage.Navigate(new ShoppingCartPage());
            }
        }

        private void SearchComputersTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchComputersTB.Text.ToLower(); // Получаем текст из TextBox и приводим к нижнему регистру для удобства поиска

            if (string.IsNullOrEmpty(searchText)) // Если строка поиска пустая, загружаем все компьютеры из базы данных
            {
                ComputersListView.ItemsSource = null;
                ComputersListView.ItemsSource = Computers;
            }
            else
            {
                Computers = new ObservableCollection<Computer>(
                    _db.Computers
                        .Where(c =>
                            c.ComputersName.ToLower().Contains(searchText) ||
                            c.ComputersComponent.ToLower().Contains(searchText) ||
                            (c.Price != null && c.Price.ToString().ToLower().Contains(searchText)))
                        .ToList());
                ComputersListView.ItemsSource = null;
                ComputersListView.ItemsSource = Computers;
            }
        }
    }


    public class CartButtonContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInCart = (bool)value;
            return isInCart ? "В корзине" : "Добавить в корзину";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CartButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isInCart = (bool)value;
            return isInCart ? Brushes.Gray : Brushes.Green;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
