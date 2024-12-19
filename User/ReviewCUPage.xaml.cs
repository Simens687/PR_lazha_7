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

namespace IS_Bibl.User
{
    /// <summary>
    /// Логика взаимодействия для ReviewCUPage.xaml
    /// </summary>
    public partial class ReviewCUPage : Page
    {
        ReviewsTableAdapter TAReviews = new ReviewsTableAdapter();
        bool IsCreating = true;
        int bookId, reviewId;



        public ReviewCUPage(int bookId, int reviewId = -1)
        {
            InitializeComponent();

            this.bookId = bookId;
            this.reviewId = reviewId;

            // Если это не создание нового отзыва, загружаем данные для редактирования
            if (reviewId != -1)
            {
                IsCreating = false;

                // Получаем отзыв по ID
                var reviews = TAReviews.GetData();
                var review = reviews.FirstOrDefault(r => r.review_id == reviewId);

                if (review != null)
                {
                    // Устанавливаем текст отзыва в TextBox
                    TbxText.Text = review.review_text;

                    // Устанавливаем рейтинг в ComboBox
                    foreach (ComboBoxItem item in CbxRating.Items)
                    {
                        if (item.Content.ToString() == review.rating.ToString())
                        {
                            CbxRating.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Отзыв не найден. Возможно, он был удалён.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    this.NavigationService.GoBack();
                }
            }
        }


        private void Exit(object sender, RoutedEventArgs e)
        {
            // Навигация назад
            this.NavigationService.GoBack();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            // Получаем текст отзыва и выбранный рейтинг
            string reviewText = TbxText.Text;
            if (!int.TryParse((CbxRating.SelectedItem as ComboBoxItem)?.Content.ToString(), out int rating))
            {
                MessageBox.Show("Выберите корректный рейтинг.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Получаем сегодняшнюю дату
            string reviewDate = DateTime.Now.ToString("yyyy-MM-dd");

            if (IsCreating)
            {
                // Вставляем новый отзыв
                TAReviews.InsertQuery(bookId, GlobalVariables.idUser, rating, reviewText, reviewDate);
                this.NavigationService.Navigate(new BookPage(bookId));
            }
            else
            {
                // Обновляем существующий отзыв
                TAReviews.UpdateQuery(bookId, GlobalVariables.idUser, rating, reviewText, reviewDate, reviewId);
                this.NavigationService.Navigate(new ReviewsPage());
            }
        }

    }
}
