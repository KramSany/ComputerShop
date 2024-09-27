using ComputerApp.Data;
using ComputerApp.Pages.AuthPage;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ComputerApp.Pages
{
    /// <summary>
    /// Логика взаимодействия для ShoppingCartPage.xaml
    /// </summary>
    public partial class ShoppingCartPage : Page
    {
        private ComputerStoreEntities _db = ComputerStoreEntities.GetContext();
        public ObservableCollection<ShoppingCart> CartItems { get; set; }

        public ShoppingCartPage()
        {
            InitializeComponent();
            LoadCartItems();
            TotalPrise.Text = "Общая сумма: " + CalculateTotalOrderPrice();
        }

        private void LoadCartItems()
        {
            var buyerId = CurrentUser.LoggedInUser.BuyerID;

            // Загружаем элементы корзины для текущего пользователя
            var cartItems = _db.ShoppingCarts
                .Include("Component") // Включаем связанные компоненты по строковому имени свойства
                .Include("Computer") // Включаем связанные компьютеры
                .Where(sc => sc.BuyerID == buyerId)
                .ToList();

            CartItems = new ObservableCollection<ShoppingCart>(cartItems);

            DataContext = this;
        }

        private void IncreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button?.DataContext as ShoppingCart;

            if (cartItem != null)
            {
                cartItem.Quantity++;
                _db.SaveChanges();
                ReloadPage(); // Для обновления данных корзины. Костыль.
            }
        }

        private void DecreaseQuantityButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var cartItem = button?.DataContext as ShoppingCart;

            if (cartItem != null && cartItem.Quantity > 1)
            {
                cartItem.Quantity--;
                _db.SaveChanges();
                ReloadPage(); // Для обновления данных корзины. Костыль.
            }
            else if (cartItem != null && cartItem.Quantity == 1)
            {
                // Если количество становится 0, удаляем элемент из корзины
                _db.ShoppingCarts.Remove(cartItem);
                _db.SaveChanges();
                ReloadPage(); // Для обновления данных корзины. Костыль.
            }
        }

        private void PlaceOrderButton_Click(object sender, RoutedEventArgs e)
        {
            // Создание нового заказа
            var newOrder = new Order
            {
                BuyersID = CurrentUser.LoggedInUser.BuyerID, // ID текущего пользователя
                OrderDate = DateTime.Now, // Текущая дата и время заказа
                OrderPrice = CalculateTotalOrderPrice(), // Общая стоимость заказа
                BuyersFIO = FullNameTextBox.Text,
                BuyersPhone = PhoneNumberTextBox.Text,
                BuyersAdress = DeliveryAddressTextBox.Text
            };

            // Добавляем созданный заказ в контекст базы данных
            _db.Orders.Add(newOrder);

            try
            {
                _db.SaveChanges();

                var buyerId = CurrentUser.LoggedInUser.BuyerID;
                var userCartItems = _db.ShoppingCarts.Where(sc => sc.BuyerID == buyerId).ToList();
                _db.ShoppingCarts.RemoveRange(userCartItems);
                _db.SaveChanges();

                MessageBox.Show("Заказ успешно оформлен!");
                
                CartItems.Clear();
                
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.FramePage.Navigate(new ComponentsPage()); // Обратно на компоненты
                    mainWindow.Height = 800;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при оформлении заказа: {ex.Message}");
            }
        }


        private decimal CalculateTotalOrderPrice()
        {
            decimal totalOrderPrice = 0;

            // Перебираем все элементы в корзине пользователя
            foreach (var cartItem in CartItems)
            {
                // Проверяем, является ли элемент компонентом
                if (cartItem.Component != null && cartItem.Component.Price.HasValue && cartItem.Quantity.HasValue)
                {
                    totalOrderPrice += cartItem.Component.Price.Value * cartItem.Quantity.Value;
                }
                // Проверяем, является ли элемент компьютером
                else if (cartItem.Computer != null && cartItem.Computer.Price.HasValue && cartItem.Quantity.HasValue)
                {
                    totalOrderPrice += cartItem.Computer.Price.Value * cartItem.Quantity.Value;
                }
            }

            return totalOrderPrice;
        }

        private void BackClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FramePage.Navigate(new ComponentsPage());
                mainWindow.Height = 800;
            }
        }

        private void ReloadPage()
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.FramePage.Navigate(new ShoppingCartPage());
            }
        }
    }
}
