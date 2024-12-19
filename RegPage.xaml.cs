using IS_Bibl.DB_BiblDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
    /// <summary>
    /// Логика взаимодействия для RegPage.xaml
    /// </summary>
    public partial class RegPage : Page
    {
        UsersTableAdapter TAUsers = new UsersTableAdapter();

        private static int Code;
        public RegPage()
        {
            InitializeComponent();
        }

        private void GoAuth(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AuthPage());
        }

        private async void GetCode(object sender, RoutedEventArgs e)
        {
            BtnGetCode.IsEnabled = false;

            GenerateCode();
            MessageBox.Show(Code.ToString());
            //SendEmail();

            TbkTimeOutCode.Text = "Новый код через 5 сек";

            await Task.Delay(1000);
            TbkTimeOutCode.Text = "Новый код через 4 сек";

            await Task.Delay(1000);
            TbkTimeOutCode.Text = "Новый код через 3 сек";

            await Task.Delay(1000);
            TbkTimeOutCode.Text = "Новый код через 2 сек";

            await Task.Delay(1000);
            TbkTimeOutCode.Text = "Новый код через 1 сек";

            await Task.Delay(1000);
            TbkTimeOutCode.Text = "Новый код через 0 сек";

            BtnGetCode.IsEnabled = true;
            TbkTimeOutCode.Text = "";
        }

        private void GenerateCode()
        {
            Random random = new Random();

            Code = random.Next(1000, 10000);
        }

        private void SendEmail()
        {
            try
            {
                string email = TbxEmail.Text;
                if (CheckEmail(email)) { }
                else { return; }

                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("isip_a.d.pitubayev@mpt.ru", "Sasha&olya"),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("isip_a.d.pitubayev@mpt.ru"),
                    Subject = "Код подтверждения",
                    Body = "Ваш код : " + Code.ToString(),
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(email);
                smtpClient.Send(mailMessage);

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке сообщения: {ex.Message}");
            }
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
            if (PbxPass.Password.Length >= 4 && PbxPass.Password == PbxPassConf.Password) { return true; }
            return false;
        }


        private bool CheckUser()
        {
            if (!CheckEmail(TbxEmail.Text)) { return false; }

            if (!IsEmailUnique(TbxEmail.Text))
            {
                MessageBox.Show("Эта почта уже зарегистрирована.");
                return false;
            }

            if (Code.ToString() != TbxCode.Text)
            {
                MessageBox.Show("Неверный код");
                return false;
            }

            if (!CheckPassword()) { return false; }
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

        private void GoReg(object sender, RoutedEventArgs e)
        {
            bool permission = CheckUser();

            if (permission)
            {
                MessageBox.Show("Вы успешно зарегистрированы!");
                TAUsers.Insert(TbxEmail.Text, HashPassword(PbxPass.Password), "user");
            }
        }
    }
}
