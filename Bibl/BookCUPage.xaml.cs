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
    /// Логика взаимодействия для BookCUPage.xaml
    /// </summary>
    public partial class BookCUPage : Page
    {
        BooksTableAdapter TABooks = new BooksTableAdapter();
        AuthorsTableAdapter TAAuthors = new AuthorsTableAdapter();
        GenresTableAdapter TAGenres = new GenresTableAdapter();

        BookAuthorsTableAdapter TABookAuth = new BookAuthorsTableAdapter();
        BookGenresTableAdapter TABookGenre = new BookGenresTableAdapter();

        bool IsCreating = true;
        int BookId = -1;
        public BookCUPage(bool isCreating, int bookId)
        {
            InitializeComponent();

            DGAuthors.ItemsSource = TAAuthors.GetData();
            DGGenres.ItemsSource = TAGenres.GetData();
            IsCreating = isCreating;
            BookId = bookId;

            if (!IsCreating)
            {
                LoadBookData(BookId);
            }
        }

        private void LoadBookData(int bookId)
        {
            // Загружаем данные о книге
            var book = TABooks.GetData().FirstOrDefault(b => b.book_id == bookId);
            if (book != null)
            {
                TbxName.Text = book["title"].ToString();
                TbxDescription.Text = book["description"].ToString();
                TbxDate.Text = book["year_published"].ToString();
                TbxQuantity.Text = book["quantity"].ToString();
            }

            // Загружаем авторов книги
            var bookAuthors = TABookAuth.GetData().Where(ba => ba.book_id == bookId).Select(ba => ba.author_id);
            foreach (var item in DGAuthors.Items)
            {
                if (item is DataRowView row && bookAuthors.Contains((int)row["author_id"]))
                {
                    DGAuthors.SelectedItems.Add(item);
                }
            }

            // Загружаем жанры книги
            var bookGenres = TABookGenre.GetData().Where(bg => bg.book_id == bookId).Select(bg => bg.genre_id);
            foreach (var item in DGGenres.Items)
            {
                if (item is DataRowView row && bookGenres.Contains((int)row["genre_id"]))
                {
                    DGGenres.SelectedItems.Add(item);
                }
            }
        }

        private void DGA_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGAuthors.Columns)
            {
                if (column.Header.ToString() == "name")
                {
                    column.Header = "Автор";
                }
                else if (column.Header.ToString() == "lifespan")
                {
                    column.Visibility = Visibility.Collapsed;
                }
                else if (column.Header.ToString() == "author_id")
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DGG_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGGenres.Columns)
            {
                if (column.Header.ToString() == "genre_name")
                {
                    column.Header = "Жанр";
                }
                else if (column.Header.ToString() == "genre_id")
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void Exit(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            if (!AreFieldsValid())
            {
                return; // Если проверка не прошла, выходим из метода
            }

            int yearPublished = int.Parse(TbxDate.Text);
            int quantity = int.Parse(TbxQuantity.Text);


            if (IsCreating)
            {
                // Создание новой книги
                TABooks.InsertQuery(TbxName.Text, TbxDescription.Text, yearPublished, quantity);

                // Получаем последний вставленный book_id
                int bookId = (int)TABooks.GetData().AsEnumerable()
                                               .OrderByDescending(row => row.Field<int>("book_id"))
                                               .First()["book_id"];

                // Добавляем авторов и жанры
                AddAuthorsAndGenres(bookId);
            }
            else
            {
                // Обновление существующей книги
                TABooks.UpdateQuery(TbxName.Text, TbxDescription.Text, yearPublished, quantity, BookId);

                // Обновляем связи с авторами
                TABookAuth.DeleteQuery(BookId);
                TABookGenre.DeleteQuery(BookId);// Удаляем старые связи
                AddAuthorsAndGenres(BookId); // Добавляем новые связи
            }

            this.NavigationService.Navigate(new BooksPage());
        }

        private void AddAuthorsAndGenres(int bookId)
        {
            // Перебор всех выбранных авторов в DataGrid
            foreach (var selectedRow in DGAuthors.SelectedItems)
            {
                DataRowView rowView = selectedRow as DataRowView;
                if (rowView != null)
                {
                    int authorId = (int)rowView["author_id"];
                    // Вставка записи в таблицу связи книга-автор
                    TABookAuth.InsertQuery(bookId, authorId);
                }
            }

            // Перебор всех выбранных жанров в DataGrid
            foreach (var selectedRow in DGGenres.SelectedItems)
            {
                DataRowView rowView = selectedRow as DataRowView;
                if (rowView != null)
                {
                    int genreId = (int)rowView["genre_id"];
                    // Вставка записи в таблицу связи книга-жанр
                    TABookGenre.InsertQuery(bookId, genreId);
                }
            }
        }


        private bool AreFieldsValid()
        {
            // Проверка текстовых полей
            if (string.IsNullOrWhiteSpace(TbxName.Text))
            {
                MessageBox.Show("Поле 'Название' должно быть заполнено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbxDescription.Text))
            {
                MessageBox.Show("Поле 'Описание' должно быть заполнено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbxDate.Text))
            {
                MessageBox.Show("Поле 'Дата публикации' должно быть заполнено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbxDate.Text) || !int.TryParse(TbxDate.Text, out int year) || year <= 0)
            {
                MessageBox.Show("Поле 'Дата публикации' должно быть заполнено и содержать корректное положительное целое число (год).", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(TbxQuantity.Text) || !int.TryParse(TbxQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Поле 'Количество' должно быть заполнено и содержать корректное положительное целое число.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DGGenres.SelectedItems.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы один жанр.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DGAuthors.SelectedItems.Count == 0)
            {
                MessageBox.Show("Необходимо выбрать хотя бы одиного автора.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

    }
}
