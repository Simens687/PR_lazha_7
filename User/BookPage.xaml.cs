using IS_Bibl.DB_BiblDataSetTableAdapters;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
    /// Логика взаимодействия для BookPage.xaml
    /// </summary>
    public partial class BookPage : Page
    {
        BooksTableAdapter TABooks = new BooksTableAdapter();
        UsersTableAdapter TAUsers = new UsersTableAdapter();
        BookAuthorsTableAdapter TABookAuthors = new BookAuthorsTableAdapter();
        BookGenresTableAdapter TABookGenres = new BookGenresTableAdapter();
        AuthorsTableAdapter TAAuthors = new AuthorsTableAdapter();
        GenresTableAdapter TAGenres = new GenresTableAdapter();
        ReviewsTableAdapter TAReviews = new ReviewsTableAdapter();

        int BookId = -1;
        public BookPage(int bookId)
        {
            InitializeComponent();

            BookId = bookId;

            LoadBookData();
            LoadReviewsDG();
            LoadReviewsPie();
        }

        private void LoadBookData()
        {
            // Шаг 1: Загружаем книгу по ID
            var book = TABooks.GetData().FirstOrDefault(b => b.book_id == BookId);
            if (book == null)
            {
                MessageBox.Show("Книга не найдена.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Шаг 2: Загружаем авторов и жанры для книги
            var authorsForBook = TABookAuthors.GetData()
                .Where(ba => ba.book_id == BookId)
                .Select(ba => TAAuthors.GetData().FirstOrDefault(a => a.author_id == ba.author_id)?.name)
                .Where(name => !string.IsNullOrEmpty(name))
                .ToList();

            var genresForBook = TABookGenres.GetData()
                .Where(bg => bg.book_id == BookId)
                .Select(bg => TAGenres.GetData().FirstOrDefault(g => g.genre_id == bg.genre_id)?.genre_name)
                .Where(genre => !string.IsNullOrEmpty(genre))
                .ToList();

            // Шаг 3: Заполняем текстовые поля
            TbxName.Text = book.title;
            TbxDescription.Text = book.description;
            TbxRating.Text = book.Isaverage_ratingNull() ? "0" : book.average_rating.ToString();
            TbxDate.Text = book.year_published.ToString();
            TbxQuantity.Text = book.quantity.ToString();

            // Шаг 4: Очищаем DataGrid перед заполнением
            DGAuthors.ItemsSource = null;
            DGGenres.ItemsSource = null;
            DGAuthors.Columns.Clear();
            DGGenres.Columns.Clear();

            // Шаг 5: Добавляем колонки
            DGAuthors.Columns.Add(new DataGridTextColumn
            {
                Header = authorsForBook.Count > 1 ? "Авторы" : "Автор",
                Binding = new Binding("Автор")
            });

            DGGenres.Columns.Add(new DataGridTextColumn
            {
                Header = genresForBook.Count > 1 ? "Жанры" : "Жанр",
                Binding = new Binding("Жанр")
            });

            // Шаг 6: Устанавливаем источник данных
            DGAuthors.ItemsSource = authorsForBook.Select(a => new { Автор = a }).ToList();
            DGGenres.ItemsSource = genresForBook.Select(g => new { Жанр = g }).ToList();
        }

        private void LoadReviewsDG()
        {
            // Получаем отзывы для текущей книги
            var reviews = (from review in TAReviews.GetData()
                           join user in TAUsers.GetData() on review.user_id equals user.user_id
                           where review.book_id == BookId
                           select new
                           {
                               Пользователь = user.email, // Отображаем почту пользователя
                               Рейтинг = review.rating,
                               Отзыв = review.review_text,
                               Дата = review.review_date.ToShortDateString()
                           }).ToList();

            DGReviews.ItemsSource = reviews; // Привязываем отзывы к DataGrid
        }


        private void LoadReviewsPie()
        {
            // Получаем все отзывы для текущей книги
            var reviews = TAReviews.GetData()
                .Where(r => r.book_id == BookId)
                .ToList();

            // Подсчитываем количество отзывов для каждого рейтинга (1-5)
            var ratingsCount = new int[5];

            foreach (var review in reviews)
            {
                if (review.rating >= 1 && review.rating <= 5)
                {
                    ratingsCount[review.rating - 1]++;
                }
            }

            // Создаем и заполняем коллекцию для диаграммы
            var chartValues = new ChartValues<int>
    {
        ratingsCount[0], // 1 звезда
        ratingsCount[1], // 2 звезды
        ratingsCount[2], // 3 звезды
        ratingsCount[3], // 4 звезды
        ratingsCount[4]  // 5 звезд
    };

            // Настройка круговой диаграммы
            RatingPieChart.Series = new SeriesCollection
    {
        new PieSeries
        {
            Title = "1 звезда",
            Values = new ChartValues<int> { ratingsCount[0] },
            PushOut = 10,
            DataLabels = true,
            LabelPoint = point => $"{point.Y}"
        },
        new PieSeries
        {
            Title = "2 звезды",
            Values = new ChartValues<int> { ratingsCount[1] },
            PushOut = 10,
            DataLabels = true,
            LabelPoint = point => $"{point.Y}"
        },
        new PieSeries
        {
            Title = "3 звезды",
            Values = new ChartValues<int> { ratingsCount[2] },
            PushOut = 10,
            DataLabels = true,
            LabelPoint = point => $"{point.Y}"
        },
        new PieSeries
        {
            Title = "4 звезды",
            Values = new ChartValues<int> { ratingsCount[3] },
            PushOut = 10,
            DataLabels = true,
            LabelPoint = point => $"{point.Y}"
        },
        new PieSeries
        {
            Title = "5 звезд",
            Values = new ChartValues<int> { ratingsCount[4] },
            PushOut = 10,
            DataLabels = true,
            LabelPoint = point => $"{point.Y}"
        }
    };
        }

        private void GoRate(object sender, RoutedEventArgs e)
        {
            // Проверяем, оставил ли пользователь уже отзыв для этой книги
            ReviewsTableAdapter TAReviews = new ReviewsTableAdapter();
            var existingReview = TAReviews.GetData()
                .FirstOrDefault(r => r.book_id == BookId && r.user_id == GlobalVariables.idUser);

            if (existingReview != null)
            {
                MessageBox.Show("Вы уже оставили отзыв для этой книги.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            this.NavigationService.Navigate(new ReviewCUPage(BookId, -1));
        }

        private void GoBack(object sender, RoutedEventArgs e)
        {
            UserWindow userWindow = new UserWindow();

            userWindow.Show();
            Window.GetWindow(this)?.Close();
        }
    }
}
