using System.Linq;
using ComputerApp.Data;
namespace ComputerApp.Pages.AuthPage
{
    /// <summary>
    /// Класс для управления входом в аккаунт.
    /// </summary>
    public static class CurrentUser
    {
        // Свойство для хранения информации о текущем пользователе
        public static Buyer LoggedInUser { get; private set; }

        // Метод для входа пользователя
        public static Buyer Login(string username, string password)
        {
            Buyer user = CheckCredentials(username, password);
            if (user != null)
            {
                LoggedInUser = user;
                return user;
            }
            else
            {
                return null;
            }
        }

        // Метод для выхода пользователя
        public static void Logout()
        {
            LoggedInUser = null; 
        }

        // Для проверки учетных данных в базе данных
        private static Buyer CheckCredentials(string username, string password)
        {
            using (var dbContext = new ComputerStoreEntities())
            {
                Buyer user = dbContext.Buyers.FirstOrDefault(u => u.Login == username && u.Password == password);
                
                return user;
            }
        }
    }



}
