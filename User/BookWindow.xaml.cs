using IS_Bibl.DB_BiblDataSetTableAdapters;
using LiveCharts.Wpf;
using LiveCharts;
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
using IS_Bibl.Bibl;

namespace IS_Bibl.User
{
    /// <summary>
    /// Логика взаимодействия для BookWindow.xaml
    /// </summary>
    public partial class BookWindow : Window
    {
        public BookWindow(int bookId)
        {
            InitializeComponent();

            MainFrame.Navigate(new BookPage(bookId));
        }
    }
}
