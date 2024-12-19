using IS_Bibl.Bibl;
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

namespace IS_Bibl.User
{
    /// <summary>
    /// Логика взаимодействия для BooksPage.xaml
    /// </summary>
    public partial class BooksPage : Page
    {
        BooksTableAdapter TABooks = new BooksTableAdapter();
        BookAuthorsTableAdapter TABookAuthors = new BookAuthorsTableAdapter();
        BookGenresTableAdapter TABookGenres = new BookGenresTableAdapter();
        AuthorsTableAdapter TAAuthors = new AuthorsTableAdapter();
        GenresTableAdapter TAGenres = new GenresTableAdapter();
        public BooksPage()
        {
            InitializeComponent();

            LoadBooks();
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGBooks.Columns)
            {
                if (column.Header.ToString() == "Title")
                {
                    column.Header = "Название";
                }
                else if (column.Header.ToString() == "Description")
                {
                    column.Header = "Описание";
                }
                else if (column.Header.ToString() == "Authors")
                {
                    column.Header = "Автор/Авторы";
                }
                else if (column.Header.ToString() == "Genres")
                {
                    column.Header = "Жанр/Жанры";
                }
                else if (column.Header.ToString() == "YearPublished")
                {
                    column.Header = "Год публикации";
                }
                else if (column.Header.ToString() == "AverageRating")
                {
                    column.Header = "Средний рейтинг";
                }
                else if (column.Header.ToString() == "Quantity")
                {
                    column.Header = "Количество";
                }
                else if (column.Header.ToString() == "BookId")
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void LoadBooks()
        {
            // Шаг 1: Загружаем все данные
            var books = TABooks.GetData();
            var bookAuthors = TABookAuthors.GetData();
            var bookGenres = TABookGenres.GetData();
            var authors = TAAuthors.GetData();
            var genres = TAGenres.GetData();

            // Шаг 2: Группируем авторов и жанры по книгам
            List<BookViewModel> bookList = new List<BookViewModel>();

            foreach (DataRow book in books.Rows)
            {
                int bookId = (int)book["book_id"];

                // Получаем авторов для книги
                var authorsForBook = bookAuthors
                    .Where(ba => ba.book_id == bookId)
                    .Select(ba => authors.FirstOrDefault(a => a.author_id == ba.author_id)?.name)
                    .Where(name => !string.IsNullOrEmpty(name));

                // Получаем жанры для книги
                var genresForBook = bookGenres
                    .Where(bg => bg.book_id == bookId)
                    .Select(bg => genres.FirstOrDefault(g => g.genre_id == bg.genre_id)?.genre_name)
                    .Where(genre => !string.IsNullOrEmpty(genre));

                // Добавляем данные в список
                bookList.Add(new BookViewModel
                {
                    BookId = bookId,
                    Title = book["title"].ToString(),
                    Description = book["description"].ToString(),
                    YearPublished = (int)book["year_published"],
                    AverageRating = book["average_rating"] != DBNull.Value ? (decimal)book["average_rating"] : 0,
                    Quantity = (int)book["quantity"],
                    Authors = string.Join(", \n", authorsForBook),
                    Genres = string.Join(", \n", genresForBook)
                });
            }

            // Шаг 3: Привязываем данные к DataGrid
            DGBooks.ItemsSource = bookList;
        }

        private void GoMore(object sender, RoutedEventArgs e)
        {
            if (DGBooks.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для обновления.", "Подробнее", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBook = (BookViewModel)DGBooks.SelectedItem;

            
           
            // Открываем окно книги
            BookWindow bookWindow = new BookWindow(selectedBook.BookId);
            bookWindow.Show();

            Window.GetWindow(this)?.Close();
        }
    }
}
