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
    /// Логика взаимодействия для BooksPage.xaml
    /// </summary>
    /// 

    public class BookViewModel
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string Authors { get; set; }
        public string Genres { get; set; }
        
        public int YearPublished { get; set; }
        public decimal AverageRating { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
    }

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

        private void Create(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BookCUPage(true, 0));
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

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGBooks.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для удаления.", "Удаление книги", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBook = (BookViewModel)DGBooks.SelectedItem; // Приводим к типу BookViewModel
            int bookId = selectedBook.BookId;

            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите удалить выбранную книгу?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TABooks.DeleteQuery(bookId); // Вызов метода удаления в адаптере
                    MessageBox.Show("Книга успешно удалена.", "Удаление книги", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновление DataGrid после удаления
                    LoadBooks(); // Перезагружаем данные в DataGrid
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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

        private void Update(object sender, RoutedEventArgs e)
        {
            if (DGBooks.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите книгу для обновления.", "Обновление книги", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBook = (BookViewModel)DGBooks.SelectedItem;
            this.NavigationService.Navigate(new BookCUPage(false, selectedBook.BookId));
        }

    }
}
