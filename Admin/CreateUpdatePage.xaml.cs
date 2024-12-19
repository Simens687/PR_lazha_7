using IS_Bibl.DB_BiblDataSetTableAdapters;
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
using static System.Runtime.CompilerServices.RuntimeHelpers;

namespace IS_Bibl.Admin
{
    public partial class CreateUpdatePage : Page
    {
        bool IsCreating = true;

        UsersTableAdapter TAUsers = new UsersTableAdapter();
        public CreateUpdatePage(bool isCreating)
        {
            InitializeComponent();

            IsCreating = isCreating;

            if (!isCreating)
            {
                PbxPass.Visibility = Visibility.Collapsed;
                TbxEmail.Text = GlobalVariables.changedUser["email"].ToString();
                TbxRole.Text = GlobalVariables.changedUser["role"].ToString();
            }
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (IsCreating)
            {
                bool permission = CheckUser();

                if (permission)
                {
                    TAUsers.Insert(TbxEmail.Text, HashPassword(PbxPass.Password), TbxRole.Text);
                    MessageBox.Show("Пользователь добавлен");
                    Exit(sender, e);
                }
            }
            else
            {
                if (CheckEmail(TbxEmail.Text)) 
                { 
                    TAUsers.UpdateQuery(TbxEmail.Text, TbxRole.Text, (int)GlobalVariables.changedUser["user_id"]);
                    MessageBox.Show("Пользователь обновлен");
                    Exit(sender, e);
                }
            }
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

        private bool CheckUser()
        {
            if (!CheckEmail(TbxEmail.Text)) { return false; }

            if (!IsEmailUnique(TbxEmail.Text))
            {
                MessageBox.Show("Эта почта уже зарегистрирована.");
                return false;
            }

            if (!CheckPassword()) { return false; }
            return true;
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

        private bool CheckPassword()
        {
            if (PbxPass.Password.Length >= 4) { return true; }
            return false;
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            AdminWindow adminWindow = new AdminWindow();
            adminWindow.Show();

            var window = Window.GetWindow(this);
            window.Close();
        }
    }
}