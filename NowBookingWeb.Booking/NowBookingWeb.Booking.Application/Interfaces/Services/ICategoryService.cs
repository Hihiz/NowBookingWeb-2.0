using NowBookingWeb.Booking.Application.Dto.Output.Category;

namespace NowBookingWeb.Booking.Application.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса категорий.
    /// </summary>
    public interface ICategoryService
    {
        /// <summary>
        /// Метод получает список категорий.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Список категорий.</returns>
        Task<IEnumerable<CategoryOutput>> GetCategoriesAsync(CancellationToken cancellationToken);
    }
}
