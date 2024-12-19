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
    public class BorrowingViewModel
    {
        public int BorrowingId { get; set; }
        public string BookTitle { get; set; }
        public string UserEmail { get; set; } // Используем почту пользователя
        public DateTime BorrowDate { get; set; }
        public DateTime? DueDate { get; set; } // Nullable DateTime
    }




    public partial class BorrowingsPage : Page
    {
        BorrowingTableAdapter TABorrowings = new BorrowingTableAdapter();
        BooksTableAdapter TABooks = new BooksTableAdapter();
        UsersTableAdapter TAUsers = new UsersTableAdapter();

        public BorrowingsPage()
        {
            InitializeComponent();
            LoadBorrowings();
        }

        private void Create(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new BorrowingCUPage(true, 0)); // Переход на страницу создания
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGBorrowings.Columns)
            {
                if (column.Header.ToString() == "BookTitle")
                {
                    column.Header = "Название книги";
                }
                else if (column.Header.ToString() == "UserEmail") // Изменено на UserEmail
                {
                    column.Header = "Email"; // Отображаем почту пользователя
                }
                else if (column.Header.ToString() == "BorrowDate")
                {
                    column.Header = "Дата выдачи";
                }
                else if (column.Header.ToString() == "DueDate")
                {
                    column.Header = "Дата возврата";
                }
            }
        }

        private void LoadBorrowings()
        {
            var borrowings = TABorrowings.GetData();
            var books = TABooks.GetData();
            var users = TAUsers.GetData();

            List<BorrowingViewModel> borrowingList = new List<BorrowingViewModel>();

            foreach (DataRow borrowing in borrowings.Rows)
            {
                int borrowingId = (int)borrowing["borrowing_id"];
                int bookId = (int)borrowing["book_id"];
                int userId = (int)borrowing["user_id"];

                var bookTitle = books.FirstOrDefault(b => (int)b["book_id"] == bookId)?["title"]?.ToString() ?? "Неизвестно";
                var issueDate = (DateTime)borrowing["issue_date"];  // Используем issue_date из БД
                var returnDate = borrowing["return_date"] != DBNull.Value ? (DateTime?)borrowing["return_date"] : null;  // Проверка на null для return_date
                var userEmail = users.FirstOrDefault(u => (int)u["user_id"] == userId)?["email"]?.ToString() ?? "Неизвестно";

                borrowingList.Add(new BorrowingViewModel
                {
                    BorrowingId = borrowingId,
                    BookTitle = bookTitle,
                    UserEmail = userEmail,  // Записываем почту пользователя
                    BorrowDate = issueDate.Date,  // Убираем время (оставляем только дату)
                    DueDate = returnDate?.Date // Если returnDate null, подставляем default(DateTime)
                });
            }
            DGBorrowings.ItemsSource = borrowingList;
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGBorrowings.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBorrowing = (BorrowingViewModel)DGBorrowings.SelectedItem;
            int borrowingId = selectedBorrowing.BorrowingId;

            MessageBoxResult result = MessageBox.Show(
                "Вы действительно хотите удалить выбранную запись?",
                "Подтверждение удаления",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question
            );

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TABorrowings.DeleteQuery(borrowingId); // Вызов метода удаления записи
                    MessageBox.Show("Запись успешно удалена.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Обновление DataGrid после удаления
                    LoadBorrowings(); // Перезагружаем данные в DataGrid
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (DGBorrowings.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для обновления.", "Обновление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedBorrowing = (BorrowingViewModel)DGBorrowings.SelectedItem;
            this.NavigationService.Navigate(new BorrowingCUPage(false, selectedBorrowing.BorrowingId)); // Переход на страницу редактирования
        }

    }
}
