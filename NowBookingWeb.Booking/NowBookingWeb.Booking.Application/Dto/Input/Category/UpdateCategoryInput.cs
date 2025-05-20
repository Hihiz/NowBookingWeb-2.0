namespace NowBookingWeb.Booking.Application.Dto.Input.Category
{
    /// <summary>
    /// Класс входной модели редактирования категории.
    /// </summary>
    public class UpdateCategoryInput
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
