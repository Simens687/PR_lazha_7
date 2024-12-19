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

namespace IS_Bibl.Bibl
{
    /// <summary>
    /// Логика взаимодействия для BiblWindow.xaml
    /// </summary>
    public partial class BiblWindow : Window
    {
        public BiblWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new AuthorsPage());
        }

        private void GoAuthors(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AuthorsPage());
        }

        private void GoGenres(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new GenresPage());
        }

        private void GoBooks(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BooksPage());
        }

        private void GoBorrowing(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new BorrowingsPage());
        }

        private void GoReservation(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ReservationsPage());
        }
    }
}
