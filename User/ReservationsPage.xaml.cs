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
    /// Логика взаимодействия для ReservationsPage.xaml
    /// </summary>
    public partial class ReservationsPage : Page
    {
        ReservationsTableAdapter TAReservations = new ReservationsTableAdapter();
        BooksTableAdapter TABooks = new BooksTableAdapter();
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

            List<ReservationViewModel> reservationList = new List<ReservationViewModel>();

            foreach (DataRow reservation in reservations.Rows)
            {
                int reservationId = (int)reservation["reservation_id"];
                int bookId = (int)reservation["book_id"];
                int userId = (int)reservation["user_id"];

                // Фильтрация только для текущего пользователя
                if (userId != GlobalVariables.idUser)
                {
                    continue;
                }

                // Получаем название книги с проверкой на null
                var bookTitle = books.FirstOrDefault(b => (int)b["book_id"] == bookId)?["title"]?.ToString();

                // Пропускаем запись, если название книги пустое
                if (string.IsNullOrEmpty(bookTitle))
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

                // Добавляем запись в список
                reservationList.Add(new ReservationViewModel
                {
                    ReservationId = reservationId,
                    BookTitle = bookTitle,
                    ReserveDate = reserveDate.Value,
                    DueDate = dueDate
                });
            }

            // Привязываем данные к DataGrid
            DGReservations.ItemsSource = reservationList;
        }

    }
}
