using ComputerApp.Data;
using System;
using System.Collections.Generic;
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

namespace ComputerApp.Pages.AuthPage
{
    /// <summary>
    /// Логика взаимодействия для RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        private ComputerStoreEntities _db = ComputerStoreEntities.GetContext();

        public RegisterPage()
        {
            InitializeComponent();
        }

        private void RegisterClick(object sender, RoutedEventArgs e)
        {
            string login = RegisterLoginTextBox.Text;
            string password = RegisterPasswordBox.Password;
            string confirmPassword = ConfirmPasswordBox.Password;

            // Проверка наличия спецсимволов и цифр в пароле
            if (!password.Any(char.IsDigit))
            {
                MessageBox.Show("Пароль должен содержать как минимум один специальный символ и одну цифру.");
                return;
            }

            // Проверка совпадения паролей
            if (password != confirmPassword)
            {
                MessageBox.Show("Пароли не совпадают.");
                return;
            }

            // Создание нового покупателя
            var newBuyer = new Buyer
            {
                Login = login,
                Password = password,
                ContactInformation = confirmPassword
            };

            // Добавление нового покупателя в базу данных
            _db.Buyers.Add(newBuyer);

            try
            {
                // Сохранение изменений в базе данных
                _db.SaveChanges();
                MessageBox.Show("Регистрация успешно завершена.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при регистрации: {ex.Message}");
            }

            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Height = 450;
                mainWindow.FramePage.Navigate(new AuthPage());
            }
        }

        // Кнопка назад к главному окну
        private void BackBtnClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Height = 450;
                mainWindow.FramePage.Navigate(new AuthPage());
            }
        }
    }
}
