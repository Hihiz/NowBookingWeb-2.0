namespace NowBookingWeb.Category.Application.Dto.Input.Category
{
    /// <summary>
    /// Класс входной модели создания категории.
    /// </summary>
    public class CreateCategoryInput
    {
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
