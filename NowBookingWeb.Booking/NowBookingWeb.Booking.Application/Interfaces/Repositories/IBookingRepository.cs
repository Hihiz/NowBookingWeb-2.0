using NowBookingWeb.Booking.Application.Dto.Output.Booking;
using NowBookingWeb.Booking.Domain.Enums;

namespace NowBookingWeb.Booking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория бронирования.
    /// </summary>
    public interface IBookingRepository
    {
        /// <summary>
        /// Метод получает список бронирований.
        /// </summary>
        /// <returns>Список бронирования.</returns>
        Task<IEnumerable<BookingOutput>> GetBookingsAsync();

        /// <summary>
        /// Метод получает бронирование по Id бронирования. 
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <returns>Бронирование.</returns>
        Task<BookingOutput> GetBookingByBookingIdAsync(int bookingId);

        /// <summary>
        /// Метод получает статус бронирования по Id бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <returns>Статус бронирования</returns>
        Task<BookingStatusEnum> GetBookingStatusByBookingIdAsync(int bookingId);

        /// <summary>
        /// Метод создает бронирование.
        /// </summary>
        /// <param name="startDate">Дата начала бронирования.</param>
        /// <param name="endDate">Дата окончания бронирования.</param>
        /// <param name="userId">Id пользователя, который бронирует.</param>
        /// <param name="categoryId">Id категории.</param>        
        /// <returns>Созданное бронирование.</returns>
        Task<BookingOutput> CreateBookingAsync(DateTime startDate, DateTime? endDate, long userId,
           int categoryId);

        /// <summary>
        /// Метод обновляет бронирование.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="startDate">Дата начала бронирования.</param>
        /// <param name="endDate">Дата окончания бронирования.</param>
        /// <param name="categoryId">Id категории.</param>
        /// <returns>Обновленное бронирование.</returns>
        Task<BookingOutput> UpdateBookingAsync(int bookingId, DateTime startDate, DateTime? endDate,
         long userId, int categoryId, BookingStatusEnum statusEnum);

        /// <summary>
        /// Метод обновляет статус бронированию.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="statusEnum">Новый статус бронирования.</param>
        /// <returns>Бронирование с обновленным статусом.</returns>
        Task<BookingOutput> ChangeBookingStatusAsync(int bookingId, BookingStatusEnum statusEnum);

        /// <summary>
        /// Метод изменяет пользователя в бронировании.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="userId">Id нового пользователя.</param>
        /// <returns>Бронирование с измененным пользователем.</returns>
        Task<BookingOutput> ChangeUserInBookingAsync(int bookingId, long userId);

        /// <summary>
        /// Метод удаляет бронирование.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <returns>Удаленное бронирование.</returns>
        Task<BookingOutput> RemoveBookingAsync(int bookingId);
    }
}
