using IS_Bibl.Admin;
using IS_Bibl.Bibl;
using IS_Bibl.DB_BiblDataSetTableAdapters;
using IS_Bibl.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
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

namespace IS_Bibl
{
    public partial class AuthPage : Page
    {
        UsersTableAdapter TAUsers = new UsersTableAdapter();
        public AuthPage()
        {
            InitializeComponent();
        }

        private void GoReg(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new RegPage());
        }

        private bool IsEmailUnique(string email)
        {
            var userTable = TAUsers.GetUserByEmail(email);

            return userTable.Rows.Count == 0;
        }

        private bool CheckEmail(string email)
        {
            string emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                MessageBox.Show("Некорректный адрес электронной почты.");
                return false;
            }
            return true;
        }

        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

                byte[] hashBytes = sha256.ComputeHash(passwordBytes);

                StringBuilder hashBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashBuilder.Append(b.ToString("x2"));
                }

                return hashBuilder.ToString();
            }
        }

        private void GoAuth(object sender, RoutedEventArgs e)
        {
            string email = TbxEmail.Text;

            if (!CheckEmail(email))
            {
                return;
            }

            if (IsEmailUnique(email)) { MessageBox.Show("Пользователь отсутствует"); return; }

            var userTable = TAUsers.GetUserByEmail(email);
            var user = userTable.Rows[0];

            if (user["email"].ToString() == email && user["password_hash"].ToString() == HashPassword(PbxPass.Password))
            {
                string userRole = user["role"].ToString();

                var nextWindow = new Window();

                switch (userRole)
                {
                    case "user":
                        MessageBox.Show("ТЫ ПОЛЬЗОВАТЕЛЬ");
                        nextWindow = new UserWindow();
                        break;
                    case "bibl":
                        MessageBox.Show("ТЫ БИБЛИОТЕКАРЬ");
                        nextWindow = new BiblWindow();
                        break;
                    case "admin":
                        MessageBox.Show("ТЫ АДМИНИСТРАТОР");
                        nextWindow = new AdminWindow();
                        break;
                }
                GlobalVariables.idUser = (int)user["user_id"];

                nextWindow.Show();

                Window.GetWindow(this)?.Close();
            }
            else
            {
                MessageBox.Show("Ошибка авторизации");
            }
        }
    }
}
