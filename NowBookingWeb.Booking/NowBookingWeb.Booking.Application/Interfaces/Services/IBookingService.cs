using NowBookingWeb.Booking.Application.Dto.Output.Booking;
using NowBookingWeb.Booking.Domain.Enums;

namespace NowBookingWeb.Booking.Application.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса бронирования.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Метод получает список бронирований.
        /// </summary>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Список бронирования.</returns>
        Task<IEnumerable<BookingOutput>> GetBookingsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод получает статус бронирования по Id бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Статус бронирования</returns>
        Task<BookingStatusEnum> GetBookingStatusByBookingIdAsync(int bookingId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод обновляет статус бронированию.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="statusEnum">Новый статус бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Бронирование с обновленным статусом.</returns>
        Task<BookingOutput> ChangeBookingStatusAsync(int bookingId, BookingStatusEnum statusEnum,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод оставляет заявку на возврат средств.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        Task RefundPaymentBookingAsync(int bookingId, CancellationToken cancellationToken = default);
    }
}
