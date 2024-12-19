using IS_Bibl.DB_BiblDataSetTableAdapters;
using System;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IS_Bibl.Bibl
{
    /// <summary>
    /// Логика взаимодействия для BorrowingCUPage.xaml
    /// </summary>
    public partial class BorrowingCUPage : Page
    {
        BorrowingTableAdapter TABorrowings = new BorrowingTableAdapter();
        BooksTableAdapter TABooks = new BooksTableAdapter();
        UsersTableAdapter TAUsers = new UsersTableAdapter();

        QueriesTableAdapter TAQueries = new QueriesTableAdapter();

        bool IsCreating = true;
        int BorrowingId = -1;

        public BorrowingCUPage(bool isCreating, int borrowingId)
        {
            InitializeComponent();

            DGBooks.ItemsSource = TABooks.GetData();  // Загружаем список книг
            DGUsers.ItemsSource = TAUsers.GetDataBy2("user");  // Загружаем список пользователей

            IsCreating = isCreating;
            BorrowingId = borrowingId;

            SetDatePickerLimits();

            DPDateS.SelectedDate = DateTime.Now;

            if (!IsCreating)
            {
                LoadBorrowingData(BorrowingId);
            }
        }

        private void LoadBorrowingData(int borrowingId)
        {
            // Загружаем данные о бронировании
            var borrowing = TABorrowings.GetData().FirstOrDefault(b => b.borrowing_id == borrowingId);
            if (borrowing != null)
            {
                DPDateS.SelectedDate = borrowing["issue_date"] as DateTime?;
                DPDateE.SelectedDate = borrowing["return_date"] as DateTime?;
            }

            // Загружаем данные о книге
            var bookId = (int)borrowing["book_id"];
            var book = TABooks.GetData().FirstOrDefault(b => b.book_id == bookId);
            if (book != null)
            {
                var selectedBook = DGBooks.Items.Cast<DataRowView>()
                    .FirstOrDefault(row => (int)row["book_id"] == bookId);

                if (selectedBook != null)
                {
                    DGBooks.SelectedItem = selectedBook;
                }
            }

            // Загружаем данные о пользователе
            var userId = (int)borrowing["user_id"];
            var user = TAUsers.GetData().FirstOrDefault(u => u.user_id == userId);
            if (user != null)
            {
                var selectedUser = DGUsers.Items.Cast<DataRowView>()
                    .FirstOrDefault(row => (int)row["user_id"] == userId);

                if (selectedUser != null)
                {
                    DGUsers.SelectedItem = selectedUser;
                }
            }
        }

        private void DGB_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGBooks.Columns)
            {
                if (column.Header.ToString() == "title")
                {
                    column.Header = "Название книги";
                }
                else
                {
                    column.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void DGU_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGUsers.Columns)
            {
                if (column.Header.ToString() == "email")
                {
                    column.Header = "Email";
                }
                else
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
            // Загружаем данные о бронировании, если это обновление
            DataRow selectedBorrowing = null;
            if (!IsCreating)
            {
                selectedBorrowing = TABorrowings.GetData().FirstOrDefault(b => b.borrowing_id == BorrowingId);
                if (selectedBorrowing == null)
                {
                    MessageBox.Show("Бронирование не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            if (!AreFieldsValid())
            {
                return; // Если проверка не прошла, выходим из метода
            }

            // Безопасное извлечение данных
            var selectedBookItem = DGBooks.SelectedItem as DataRowView;
            var selectedUserItem = DGUsers.SelectedItem as DataRowView;

            if (selectedBookItem == null || selectedUserItem == null)
            {
                MessageBox.Show("Необходимо выбрать книгу и пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int bookId = Convert.ToInt32(selectedBookItem["book_id"]);
            int userId = Convert.ToInt32(selectedUserItem["user_id"]);
            DateTime startDate = DPDateS.SelectedDate ?? DateTime.Now;
            DateTime? endDate = DPDateE.SelectedDate; // Nullable DateTime для даты возврата

            // Преобразуем дату выдачи в строку в формате "yyyy-MM-dd"
            string startDateString = startDate.ToString("yyyy-MM-dd");
            // Преобразуем дату возврата (если она есть)
            string endDateString = endDate.HasValue ? endDate.Value.ToString("yyyy-MM-dd") : null;

            // Если это обновление, проверяем, нужно ли вызывать процедуры
            if (!IsCreating)
            {
                // Извлекаем дату возврата, проверяя на DBNull
                DateTime? previousEndDate = selectedBorrowing["return_date"] != DBNull.Value
                                            ? (DateTime?)selectedBorrowing["return_date"]
                                            : null;

                // Если дата возврата была, а теперь её нет
                if (previousEndDate.HasValue && !endDate.HasValue)
                {
                    // Логика для возврата книги
                    TAQueries.BorrowBook(bookId);
                    
                }
                // Если даты возврата не было, а теперь она есть
                else if (!previousEndDate.HasValue && endDate.HasValue)
                {
                    // Логика для выдачи книги
                    TAQueries.ReturnBook((int)selectedBorrowing["borrowing_id"]);
                }

            }
            else
            {
                try
                {
                    // Если это создание нового бронирования и дата возврата не указана
                    if (!endDate.HasValue)
                    {
                        // Вызываем процедуру BorrowBook для создания бронирования
                        TAQueries.BorrowBook(bookId);
                    }
                } catch 
                {
                    MessageBox.Show("Невозможно создать бронирование, так как количество книг равно нулю.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
            }

            // Если это создание нового бронирования или обновление с корректировкой дат
            if (IsCreating)
            {
                // Создание нового бронирования
                TABorrowings.InsertQuery(bookId, userId, startDateString, endDateString);
            }
            else
            {
                // Обновление существующего бронирования
                TABorrowings.UpdateQuery(bookId, userId, startDateString, endDateString, BorrowingId);
            }

            this.NavigationService.Navigate(new BorrowingsPage());
        }



        private bool AreFieldsValid()
        {
            // Проверка на наличие выбранной книги и пользователя
            if (DGBooks.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать книгу.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (DGUsers.SelectedItem == null)
            {
                MessageBox.Show("Необходимо выбрать пользователя.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Проверка на даты
            if (!DPDateS.SelectedDate.HasValue)
            {
                MessageBox.Show("Поле 'Дата выдачи' должно быть заполнено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        private bool ValidateDates(DateTime borrowDate, DateTime? dueDate)
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Дата выдачи может быть в пределах последней недели до завтрашнего дня
            if (borrowDate < today.AddDays(-7) || borrowDate > tomorrow)
            {
                MessageBox.Show("Дата выдачи должна быть в пределах от последней недели до завтрашнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            // Дата возврата от следующего дня после выдачи до завтрашнего дня
            if (dueDate.HasValue && (dueDate < borrowDate.AddDays(1) || dueDate > tomorrow))
            {
                MessageBox.Show("Дата возврата должна быть в пределах от следующего дня после выдачи до завтрашнего дня.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }
        private void SetDatePickerLimits()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Устанавливаем диапазоны для DPDateS (дата выдачи)
            DPDateS.DisplayDateStart = today.AddDays(-7); // Неделя назад
            DPDateS.DisplayDateEnd = tomorrow;           // Завтрашний день

            // Устанавливаем диапазоны для DPDateE (дата возврата)
            if (DPDateS.SelectedDate.HasValue)
            {
                DPDateE.DisplayDateStart = DPDateS.SelectedDate.Value.AddDays(1); // Следующий день после выдачи
            }
            else
            {
                DPDateE.DisplayDateStart = tomorrow; // Минимум - завтрашний день, если дата выдачи не выбрана
            }

            DPDateE.DisplayDateEnd = tomorrow; // Максимум - завтрашний день
        }
        private void DPDateS_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            SetDatePickerLimits(); // Перенастраиваем диапазон для DPDateE
        }

    }
}