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

namespace IS_Bibl.User
{
    /// <summary>
    /// Логика взаимодействия для UserWindow.xaml
    /// </summary>
    public partial class UserWindow : Window
    {
        public UserWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new BooksPage());
        }

        private void GoBooks(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BooksPage());
        }

        private void GoReserv(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservationsPage());
        }

        private void GoReviews(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReviewsPage());
        }
    }
}
