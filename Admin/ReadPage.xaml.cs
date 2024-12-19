using IS_Bibl.DB_BiblDataSetTableAdapters;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace IS_Bibl.Admin
{
    /// <summary>
    /// Логика взаимодействия для ReadPage.xaml
    /// </summary>
    public partial class ReadPage : Page
    {
        UsersTableAdapter TAUsers = new UsersTableAdapter();
        public ReadPage()
        {
            InitializeComponent();

            DGUsers.ItemsSource = TAUsers.GetData();


        }

        private void DGUsers_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGUsers.Columns)
            {
                if (column.Header.ToString() == "email")
                {
                    column.Header = "Email";  // Переименование столбца email
                }
                else if (column.Header.ToString() == "role")
                {
                    column.Header = "Роль";   // Переименование столбца role
                }
                else if (column.Header.ToString() == "password_hash")
                {
                    column.Visibility = Visibility.Collapsed;  // Скрытие столбца password_hash
                }
            }
        }

        private void DGUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGUsers.SelectedItem != null)
            {
                var selectedRow = DGUsers.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    int userId = (int)selectedRow["user_id"];
                    GlobalVariables.idChangedUser=userId;
                    GlobalVariables.changedUser = selectedRow;
                }
            }
        }
    }
}
