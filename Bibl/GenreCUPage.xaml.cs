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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace IS_Bibl.Bibl
{
    public partial class GenreCUPage : Page
    {
        bool IsCreating = true;
        GenresTableAdapter TAGenres = new GenresTableAdapter();
        public GenreCUPage(bool isCreating)
        {
            InitializeComponent();

            this.IsCreating = isCreating;

            if (!IsCreating)
            {
                TbxName.Text = GlobalVariables.changedGenre["genre_name"].ToString();
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (IsCreating)
            {
                TAGenres.InsertQuery(TbxName.Text.ToString());
            }
            else
            {
                TAGenres.UpdateQuery(TbxName.Text.ToString(), GlobalVariables.idChangedAuthor);
            }

            this.NavigationService.Navigate(new GenresPage());
        }
    }
}
