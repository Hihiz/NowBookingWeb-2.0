using NowBookingWeb.Category.Application.Dto.Output.Category;

namespace NowBookingWeb.Category.Application.Interfaces.Repositories
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
        /// Метод удаляет категорию.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <returns>Удаленная категория.</returns>
        Task<CategoryOutput> RemoveCategoryAsync(int categoryId);
    }
}
