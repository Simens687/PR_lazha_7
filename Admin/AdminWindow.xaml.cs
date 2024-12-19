using IS_Bibl.DB_BiblDataSetTableAdapters;
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
using System.Windows.Shapes;

namespace IS_Bibl.Admin
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        UsersTableAdapter TAUsers = new UsersTableAdapter();
        public AdminWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new ReadPage());
        }

        private void OpenCreate(object sender, RoutedEventArgs e)
        {
            BtnDelete.IsEnabled = false;
            BtnUpdate.IsEnabled = false;
            MainFrame.Navigate(new CreateUpdatePage(true));
        }

        private void OpenUpdate(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedUser == 0)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }
            else
            {
                MainFrame.Navigate(new CreateUpdatePage(false));
            }

            BtnDelete.IsEnabled = false;
            BtnCreate.IsEnabled = false;

        }

        private void DeleteUser(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedUser == 0)
            {
                MessageBox.Show("Выберите пользователя!");
                return;
            }
            else
            {
                TAUsers.DeleteQuery(GlobalVariables.idChangedUser);
                MessageBox.Show("Пользователь удален");

                MainFrame.Navigate(new ReadPage());
            }
        }
    }
}
