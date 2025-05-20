using NowBookingWeb.Booking.Application.Dto.Output.Category;

namespace NowBookingWeb.Booking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория категорий.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Метод получает список категорий.
        /// </summary>
        /// <returns>Список категорий.</returns>
        Task<IEnumerable<CategoryOutput>> GetCategoriesAsync();

        /// <summary>
        /// Метод получает категорию по Id.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <returns>Категория.</returns>
        Task<CategoryOutput> GetCategoryByCategoryIdAsync(int categoryId);

        /// <summary>
        /// Метод создает категорию.
        /// </summary>
        /// <param name="name">Название категории.</param>
        /// <param name="description">Описание категории.</param>
        /// <returns>Созданная категория.</returns>
        Task<CategoryOutput> CreateCategoryAsync(string? name, string? description);

        /// <summary>
        /// Метод редактирует категорию.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <param name="name">Название категории.</param>
        /// <param name="description">Описание категории.</param>
        /// <returns>Обновленная категория.</returns>
        Task<CategoryOutput> UpdateCategoryAsync(int categoryId, string? name, string? description);

        /// <summary>
        /// Метод удаляет категорию.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <returns>Удаленная категория.</returns>
        Task<CategoryOutput> RemoveCategoryAsync(int categoryId);
    }
}
