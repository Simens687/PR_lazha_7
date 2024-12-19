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
    /// <summary>
    /// Логика взаимодействия для AuthorCUPage.xaml
    /// </summary>
    public partial class AuthorCUPage : Page
    {
        AuthorsTableAdapter TAAuthors = new AuthorsTableAdapter();
        bool IsCreating = true;
        public AuthorCUPage(bool isCreating)
        {
            InitializeComponent();

            IsCreating = isCreating;

            if (!IsCreating)
            {
                TbxName.Text = GlobalVariables.changedAuthor["name"].ToString();
                TbxLifeSpan.Text = GlobalVariables.changedAuthor["lifespan"].ToString();
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
                TAAuthors.InsertQuery(TbxName.Text.ToString(), TbxLifeSpan.Text.ToString());
            }
            else
            {
                TAAuthors.UpdateQuery(TbxName.Text.ToString(), TbxLifeSpan.Text.ToString(), GlobalVariables.idChangedAuthor);
            }

            this.NavigationService.Navigate(new AuthorsPage());
        }
    }
}
