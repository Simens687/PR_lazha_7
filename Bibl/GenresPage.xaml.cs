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

namespace IS_Bibl.Bibl
{
    /// <summary>
    /// Логика взаимодействия для GenresPage.xaml
    /// </summary>
    public partial class GenresPage : Page
    {
        GenresTableAdapter TAGenres = new GenresTableAdapter();
        public GenresPage()
        {
            InitializeComponent();

            DGGenres.ItemsSource = TAGenres.GetData();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new GenreCUPage(true));
        }

        private void DG_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGGenres.Columns)
            {
                if (column.Header.ToString() == "genre_name")
                {
                    column.Header = "Название";
                }
                else if (column.Header.ToString() == "genre_id")
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGGenres.SelectedItem != null)
            {
                var selectedRow = DGGenres.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    int genreId = (int)selectedRow["genre_id"];
                    GlobalVariables.idChangedGenre = genreId;
                    GlobalVariables.changedGenre= selectedRow;
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedGenre == 0)
            {
                MessageBox.Show("Выберите автора!");
                return;
            }
            else
            {
                this.NavigationService.Navigate(new GenreCUPage(false));
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedGenre == 0)
            {
                MessageBox.Show("Выберите автора!");
                return;
            }
            else
            {
                TAGenres.DeleteQuery(GlobalVariables.idChangedGenre);
                this.NavigationService.Navigate(new GenresPage());
            }
        }

        
    }
}
