namespace NowBookingWeb.Category.Client
{
    /// <summary>
    /// Интерфейс клиента микросервиса категорий.
    /// </summary>
    public interface ICategoryServiceClient
    {
        /// <summary>
        /// Метод удаляет категорию.
        /// </summary>
        /// <param name="categoryId">Id категории.</param>
        /// <returns>Удаленная категория.</returns>
        Task<CategoryOutput> RemoveCategoryAsync(int categoryId, CancellationToken cancellationToken);
    }
}
