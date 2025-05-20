using NowBookingWeb.Booking.Domain.Enums;

namespace NowBookingWeb.Booking.Application.Dto.Output.Booking
{
    /// <summary>
    /// Класс выходной модели бронирования.
    /// </summary>
    public class BookingOutput
    {
        /// <summary>
        /// Id бронирования.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дата начала бронирования.
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Дата окончания бронирования.
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// Id пользователя, который бронирует.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? UserFirstName { get; set; }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string? UserLastName { get; set; }

        /// <summary>
        /// Email пользователя.
        /// </summary>
        public string? UserEmail { get; set; }

        /// <summary>
        /// Id категории.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// Статус бронирования в значении енамки.
        /// </summary>
        public BookingStatusEnum? StatusEnum { get; set; }

        /// <summary>
        /// Статус бронирования.
        /// </summary>
        public string? StatusName { get; set; }

        /// <summary>
        /// Дата создания заявки на бронирование.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
