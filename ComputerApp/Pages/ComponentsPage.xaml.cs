using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using ComputerApp.Data;
using ComputerApp.Pages.AuthPage;

namespace ComputerApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ComponentsPage.xaml 
    /// </summary>
    public partial class ComponentsPage : Page
    {
        private ComputerStoreEntities _db = ComputerStoreEntities.GetContext();
        public ObservableCollection<Component> Components { get; set; }

        public ComponentsPage()
        {
            InitializeComponent();
            _db.Components.Load();
            _db.ShoppingCarts.Load();
            _db.Buyers.Load();

            LoadComponentsFromDatabase();
        }

        private void LoadComponentsFromDatabase()
        {
            var userCartComponents = _db.ShoppingCarts
                                        .Where(cart => cart.BuyerID == CurrentUser.LoggedInUser.BuyerID)
                                        .Select(cart => cart.ComponentID)
                                        .ToList();

            Components = new ObservableCollection<Component>(_db.Components.ToList());

            // Устанавливаем состояние IsInCart для каждого компонента
            foreach (var component in Components)
            {
                component.IsInCart = userCartComponents.Contains(component.ComponentsID);
            }

            DataContext = this;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var component = button?.DataContext as Component;

            if (component != null)
            {
                var userCart = _db.ShoppingCarts
                                  .FirstOrDefault(cart => cart.BuyerID == CurrentUser.LoggedInUser.BuyerID && cart.ComponentID == component.ComponentsID);

                if (userCart != null)
                {
                    // Компонент уже в корзине, удаляем его
                    _db.ShoppingCarts.Remove(userCart);
                    component.IsInCart = false;
                }
                else
                {
                    // Компонента нет в корзине, добавляем его
                    userCart = new ShoppingCart
                    {
                        BuyerID = CurrentUser.LoggedInUser.BuyerID,
                        ComponentID = component.ComponentsID
                    };

                    _db.ShoppingCarts.Add(userCart);
                    component.IsInCart = true;
                }

                _db.SaveChanges();
                // Обновляем коллекцию компонентов
                ReloadPage();
            }
        }

        private void ReloadPage()
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null) mainWindow.FramePage.Navigate(new ComponentsPage());
        }

        // Выход из аккаунта юзера и переход на окно авторизации с изменение видимости страниц
        private void LogoutAccClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                CurrentUser.Logout();
                mainWindow.FramePage.Navigate(new AuthPage.AuthPage());
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

        private void SearchComponentsTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            string searchText = SearchComponentsTB.Text.ToLower(); // Получаем текст из TextBox и приводим к нижнему регистру для удобства поиска

            if (string.IsNullOrEmpty(searchText)) // Если строка поиска пустая, загружаем все компоненты из базы данных
            {
                ReloadPage();
            }
            else
            {
                Components = new ObservableCollection<Component>(
                    _db.Components
                        .Where(c =>
                            c.Name.ToLower().Contains(searchText) ||
                            c.Manufacturer.ToLower().Contains(searchText) ||
                            (c.Price != null && c.Price.ToString().ToLower().Contains(searchText)))
                        .ToList());
                ComponentsListView.ItemsSource = null;
                ComponentsListView.ItemsSource = Components;
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
