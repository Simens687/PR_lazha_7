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
    public class ReservationViewModel
    {
        public int ReservationId { get; set; }
        public string BookTitle { get; set; }
        public string UserEmail { get; set; }
        public DateTime ReserveDate { get; set; }
        public DateTime DueDate { get; set; }
    }

    public partial class ReservationsPage : Page
    {
        ReservationsTableAdapter TAReservations = new ReservationsTableAdapter();
        BooksTableAdapter TABooks = new BooksTableAdapter();
        UsersTableAdapter TAUsers = new UsersTableAdapter();

        public ReservationsPage()
        {
            InitializeComponent();
            LoadReservations();
        }

        private void Loaded(object sender, RoutedEventArgs e)
        {
            foreach (var column in DGReservations.Columns)
            {
                if (column.Header.ToString() == "BookTitle")
                    column.Header = "Название книги";
                else if (column.Header.ToString() == "UserEmail")
                    column.Header = "Email пользователя";
                else if (column.Header.ToString() == "ReserveDate")
                    column.Header = "Дата бронирования";
                else if (column.Header.ToString() == "DueDate")
                    column.Header = "Дата окончания брони";
            }
        }

        private void LoadReservations()
        {
            var reservations = TAReservations.GetData();
            var books = TABooks.GetData();
            var users = TAUsers.GetData();

            List<ReservationViewModel> reservationList = new List<ReservationViewModel>();

            foreach (DataRow reservation in reservations.Rows)
            {
                int reservationId = (int)reservation["reservation_id"];
                int bookId = (int)reservation["book_id"];
                int userId = (int)reservation["user_id"];

                // Получаем название книги и email пользователя с проверкой на null
                var bookTitle = books.FirstOrDefault(b => (int)b["book_id"] == bookId)?["title"]?.ToString();
                var userEmail = users.FirstOrDefault(u => (int)u["user_id"] == userId)?["email"]?.ToString();

                // Проверка, если bookTitle или userEmail пустые, то пропускаем эту запись
                if (string.IsNullOrEmpty(bookTitle) || string.IsNullOrEmpty(userEmail))
                {
                    continue;
                }

                // Дата бронирования
                var reserveDate = reservation["reservation_date"] as DateTime?;

                // Если дата бронирования пустая или некорректная, пропускаем эту запись
                if (!reserveDate.HasValue)
                {
                    continue;
                }

                // Вычисляем дату окончания бронирования (2 дня после бронирования)
                var dueDate = reserveDate.Value.AddDays(2);

                // Добавляем запись в список, если все проверки прошли успешно
                reservationList.Add(new ReservationViewModel
                {
                    ReservationId = reservationId,
                    BookTitle = bookTitle,
                    UserEmail = userEmail,
                    ReserveDate = reserveDate.Value,
                    DueDate = dueDate
                });
            }

            // Привязываем данные к DataGrid
            DGReservations.ItemsSource = reservationList;
        }



        private void Create(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new ReservationCUPage(true, 0)); // Переход на страницу создания
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            if (DGReservations.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для обновления.", "Обновление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedReservation = (ReservationViewModel)DGReservations.SelectedItem;
            this.NavigationService.Navigate(new ReservationCUPage(false, selectedReservation.ReservationId));
        }

        private void Delete(object sender, RoutedEventArgs e)
        {
            if (DGReservations.SelectedItem == null)
            {
                MessageBox.Show("Пожалуйста, выберите запись для удаления.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var selectedReservation = (ReservationViewModel)DGReservations.SelectedItem;
            int reservationId = selectedReservation.ReservationId;

            MessageBoxResult result = MessageBox.Show("Вы действительно хотите удалить выбранную запись?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    TAReservations.DeleteQuery(reservationId);
                    MessageBox.Show("Запись успешно удалена.", "Удаление записи", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadReservations();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при удалении записи: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
