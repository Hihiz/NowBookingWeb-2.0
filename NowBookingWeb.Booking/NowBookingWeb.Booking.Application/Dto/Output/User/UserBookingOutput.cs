namespace NowBookingWeb.Booking.Application.Dto.Output.User
{
    /// <summary>
    /// Класс выходной модели пользователя в бронировании.
    /// </summary>
    public class UserBookingOutput
    {
        /// <summary>
        /// Id пользователя.
        /// </summary>
        public long UserId { get; set; }

        /// <summary>
        /// Имя пользователя.
        /// </summary>
        public string? FirstName { get; set; }

        /// <summary>
        /// Фамилия пользователя.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Почта пользователя.
        /// </summary>
        public string? Email { get; set; }
    }
}
