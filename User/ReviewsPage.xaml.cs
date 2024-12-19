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
    /// Логика взаимодействия для ReviewsPage.xaml
    /// </summary>

    public class ReviewViewModel
    {
        public int ReviewId { get; set; }
        public string BookTitle { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public DateTime ReviewDate { get; set; }
    }


    public partial class ReviewsPage : Page
    {
        BooksTableAdapter TABooks = new BooksTableAdapter();
        ReviewsTableAdapter TAReviews = new ReviewsTableAdapter();
        int idBook;
        public ReviewsPage()
        {
            InitializeComponent();
        }

        private void LoadReviews(object sender, RoutedEventArgs e)
        {
            var reviews = TAReviews.GetData(); // Получаем данные отзывов
            var books = TABooks.GetData(); // Получаем данные книг

            List<ReviewViewModel> reviewList = new List<ReviewViewModel>();

            foreach (DataRow review in reviews.Rows)
            {
                int userId = (int)review["user_id"];
                int bookId = (int)review["book_id"];
                idBook = bookId;

                // Фильтрация: только отзывы текущего пользователя
                if (userId != GlobalVariables.idUser)
                {
                    continue;
                }

                // Получаем название книги
                var bookTitle = books.FirstOrDefault(b => (int)b["book_id"] == bookId)?["title"]?.ToString();

                // Проверка на пустое название книги
                if (string.IsNullOrEmpty(bookTitle))
                {
                    continue;
                }

                // Добавляем отзыв в список
                reviewList.Add(new ReviewViewModel
                {
                    ReviewId = (int)review["review_id"],
                    BookTitle = bookTitle,
                    Rating = (int)review["rating"],
                    ReviewText = review["review_text"]?.ToString(),
                    ReviewDate = (DateTime)review["review_date"]
                });
            }

            // Привязываем данные к DataGrid
            DGReviews.ItemsSource = reviewList;
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (DGReviews.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите отзыв для обновления.", "Обновление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedReservation = (ReviewViewModel)DGReviews.SelectedItem;
            this.NavigationService.Navigate(new ReviewCUPage(idBook, selectedReservation.ReviewId));
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGReviews.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedReservation = (ReviewViewModel)DGReviews.SelectedItem;
            int reservationId = selectedReservation.ReviewId;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TAReviews.DeleteQuery(reservationId);
                    MessageBox.Show("Запись успешно удалена.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadReviews(sender, e);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
