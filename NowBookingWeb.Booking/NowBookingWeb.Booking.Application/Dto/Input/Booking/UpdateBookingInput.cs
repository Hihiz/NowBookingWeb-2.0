using NowBookingWeb.Booking.Domain.Enums;

namespace NowBookingWeb.Booking.Application.Dto.Input.Booking
{
    /// <summary>
    /// Класс входной модели редактирования бронирования.
    /// </summary>
    public class UpdateBookingInput
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
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Id пользователя, который бронирует.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Id категории.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Тип статуса енамки.
        /// </summary>
        public BookingStatusEnum StatusEnum { get; set; }
    }
}
