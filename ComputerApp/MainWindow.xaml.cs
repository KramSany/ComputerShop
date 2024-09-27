using ComputerApp.Pages;
using ComputerApp.Pages.AuthPage;
using System.Windows;

namespace ComputerApp
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            FramePage.Navigate(new AuthPage());
        }

        private void ComponentListClick(object sender, RoutedEventArgs e)
        {
            FramePage.Navigate(new ComponentsPage());
        }

        private void ComputersListClick(object sender, RoutedEventArgs e)
        {
            FramePage.Navigate(new ComputersPage());
        }
    }
}
