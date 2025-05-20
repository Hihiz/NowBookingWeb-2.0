namespace NowBookingWeb.Booking.Application.Dto.Output.Category
{
    /// <summary>
    /// Класс выходной модели категории.
    /// </summary>
    public class CategoryOutput
    {
        /// <summary>
        /// Id категории.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Описание категории.
        /// </summary>
        public string? Description { get; set; }
    }
}
