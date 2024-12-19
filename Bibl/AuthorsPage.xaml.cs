using IS_Bibl.Admin;
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
    /// Логика взаимодействия для AuthorsPage.xaml
    /// </summary>
    public partial class AuthorsPage : Page
    {
        AuthorsTableAdapter TAAuthors = new AuthorsTableAdapter();
        public AuthorsPage()
        {
            InitializeComponent();

            DGAuthors.ItemsSource = TAAuthors.GetData();
        }

        private void DG_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGAuthors.Columns)
            {
                if (column.Header.ToString() == "name")
                {
                    column.Header = "Имя";  // Переименование столбца email
                }
                else if (column.Header.ToString() == "lifespan")
                {
                    column.Header = "Годы жизни";   // Переименование столбца role
                }
                else if (column.Header.ToString() == "author_id")
                {
                    column.Visibility = Visibility.Collapsed;  // Скрытие столбца password_hash
                }
            }
        }

        private void Go_Create(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new AuthorCUPage(true));
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DGAuthors.SelectedItem != null)
            {
                var selectedRow = DGAuthors.SelectedItem as DataRowView;

                if (selectedRow != null)
                {
                    int authorId = (int)selectedRow["author_id"];
                    GlobalVariables.idChangedAuthor = authorId;
                    GlobalVariables.changedAuthor = selectedRow;
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedAuthor == 0)
            {
                MessageBox.Show("Выберите автора!");
                return;
            }
            else
            {
                this.NavigationService.Navigate(new AuthorCUPage(false));
            }
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (GlobalVariables.idChangedAuthor == 0)
            {
                MessageBox.Show("Выберите автора!");
                return;
            }
            else
            {
                TAAuthors.DeleteQuery(GlobalVariables.idChangedAuthor);
                this.NavigationService.Navigate(new AuthorsPage());
            }
        }
    }
}
