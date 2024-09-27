using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ComputerApp.Pages.AuthPage
{
    /// <summary>
    /// Логика взаимодействия для AuthPage.xaml
    /// </summary>
    public partial class AuthPage : Page
    {
        public AuthPage()
        {
            InitializeComponent();
            
        }

        private void LoignClick(object sender, RoutedEventArgs e)
        {
            CurrentUser.Login(LoginTextBox.Text, PasswordBox.Password);
            if (CurrentUser.LoggedInUser == null)
            {
                ErrorMessage.Visibility = Visibility.Visible;
            }
            else
            {
                MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
                if (mainWindow != null)
                {
                    mainWindow.Height = 800;
                    mainWindow.ComponentListBtn.Visibility = Visibility.Visible;
                    mainWindow.ComputersListBtn.Visibility = Visibility.Visible;
                    mainWindow.FramePage.Navigate(new ComponentsPage());

                }
            }
            
            
        }

        private void RegisrtClick(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = Window.GetWindow(this) as MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Height = 550;
                mainWindow.FramePage.Navigate(new RegisterPage());

            }
        }
    }
}
