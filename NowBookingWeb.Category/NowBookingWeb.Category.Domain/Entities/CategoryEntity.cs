namespace NowBookingWeb.Category.Domain.Entities
{
    /// <summary>
    /// Класс сущности категории.
    /// </summary>
    public class CategoryEntity
    {
        /// <summary>
        /// Id категории.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Название категории.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Описание категории.
        /// </summary>
        public string? Description { get; set; }
    }
}
