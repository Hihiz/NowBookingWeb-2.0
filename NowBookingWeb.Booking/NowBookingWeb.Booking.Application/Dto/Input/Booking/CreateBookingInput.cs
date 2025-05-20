namespace NowBookingWeb.Booking.Application.Dto.Input.Booking
{
    /// <summary>
    /// Класс входной модели создания бронирования.
    /// </summary>
    public class CreateBookingInput
    {
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
        public long UserId { get; set; }

        /// <summary>
        /// Id категории.
        /// </summary>
        public int CategoryId { get; set; }
    }
}
