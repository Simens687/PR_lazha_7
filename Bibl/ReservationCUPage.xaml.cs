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
    /// Логика взаимодействия для ReservationCUPage.xaml
    /// </summary>
    public partial class ReservationCUPage : Page
    {
        // Адаптеры для работы с таблицами
        ReservationsTableAdapter TAReservations = new ReservationsTableAdapter();
        BooksTableAdapter TABooks = new BooksTableAdapter();
        UsersTableAdapter TAUsers = new UsersTableAdapter();

        QueriesTableAdapter TAQueries = new QueriesTableAdapter();

        bool IsCreating = true;
        int ReservationId = -1;

        public ReservationCUPage(bool isCreating, int reservationId)
        {
            InitializeComponent();

            // Загружаем данные для выбора книги и пользователя
            DGBooks.ItemsSource = TABooks.GetData();
            DGUsers.ItemsSource = TAUsers.GetDataBy2("user");

            IsCreating = isCreating;
            ReservationId = reservationId;

            SetDatePickerLimits();

            DPDateS.SelectedDate = DateTime.Now;

            if (!IsCreating)
            {
                LoadReservationData(ReservationId);
            }
        }

        private void LoadReservationData(int reservationId)
        {
            // Загружаем данные о резервировании
            var reservation = TAReservations.GetData().FirstOrDefault(r => r.reservation_id == reservationId);
            if (reservation != null)
            {
                DPDateS.SelectedDate = reservation["reservation_date"] as DateTime?;

                // Дата окончания бронирования не доступна для изменения
                DPDateE.SelectedDate = reservation["reservation_end_date"] as DateTime?;
            }

            // Загружаем данные о книге
            var bookId = (int)reservation["book_id"];
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
            var userId = (int)reservation["user_id"];
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
            // Загружаем данные о резервировании, если это обновление
            DataRow selectedReservation = null;
            if (!IsCreating)
            {
                selectedReservation = TAReservations.GetData().FirstOrDefault(r => r.reservation_id == ReservationId);
                if (selectedReservation == null)
                {
                    MessageBox.Show("Резервирование не найдено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
            DateTime? startDate = DPDateS.SelectedDate; // Используем DateTime?

            // Автоматически устанавливаем дату окончания на 2 дня позже начала
            DateTime? endDate = startDate?.AddDays(2);

            // Преобразуем дату начала и окончания в строки в формате "yyyy-MM-dd"
            string startDateString = startDate?.ToString("yyyy-MM-dd");
            string endDateString = endDate?.ToString("yyyy-MM-dd");

            // Логика для обновления/создания
            if (!IsCreating)
            {
                // Извлекаем дату окончания для текущего резервирования
                DateTime? previousEndDate = selectedReservation["reservation_end_date"] != DBNull.Value
                                                ? (DateTime?)selectedReservation["reservation_end_date"]
                                                : null;

                // Проверяем, если дата окончания уже наступила
                if (previousEndDate.HasValue && previousEndDate.Value < DateTime.Now)
                {
                    // Если дата окончания уже прошла, возвращаем книгу
                    try
                    {
                        TAQueries.CancelReservation(bookId); // Возвращаем книгу, увеличиваем количество
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при отмене резервирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // Если это создание нового резервирования, то уменьшение количества книг
            if (IsCreating)
            {
                try
                {
                    // Уменьшаем количество книг при создании резервирования
                    TAQueries.ReserveBook(bookId); // Уменьшаем количество книг
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при резервировании книги: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                // Проверяем, если дата начала была изменена и теперь дата окончания уже прошла
                DateTime? newEndDate = startDate?.AddDays(2); // Пересчитываем дату окончания на основе новой даты начала
                if (newEndDate.HasValue && newEndDate.Value < DateTime.Now)
                {
                    try
                    {
                        // Если дата начала была изменена так, что дата окончания уже прошла, добавляем книгу обратно
                        TAQueries.CancelReservation(bookId); // Возвращаем книгу
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ошибка при отмене резервирования: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            // После обработки количества книг, сохраняем/обновляем данные о резервировании
            if (IsCreating)
            {
                // Создание нового резервирования
                TAReservations.InsertQuery(bookId, userId, startDateString, endDateString);
            }
            else
            {
                // Обновление существующего резервирования
                TAReservations.UpdateQuery(bookId, userId, startDateString, endDateString, ReservationId);
            }

            // Переход назад на страницу с резервированием
            this.NavigationService.Navigate(new ReservationsPage());
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
                MessageBox.Show("Поле 'Дата начала' должно быть заполнено.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SetDatePickerLimits()
        {
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Устанавливаем диапазоны для DPDateS (дата начала)
            DPDateS.DisplayDateStart = today.AddDays(-7); // Неделя назад
            DPDateS.DisplayDateEnd = tomorrow;           // Завтрашний день

            // Убираем возможность редактирования DPDateE
            DPDateE.IsEnabled = false; // Дата окончания только автоматически устанавливается

            DPDateE.SelectedDate = DPDateS.SelectedDate?.AddDays(2); // Автоматически устанавливаем на 2 дня больше
        }

        private void DPDateS_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            // Автоматически обновляем дату окончания на 2 дня позже
            if (DPDateS.SelectedDate.HasValue)
            {
                DPDateE.SelectedDate = DPDateS.SelectedDate.Value.AddDays(2);
            }
        }
    }

}
