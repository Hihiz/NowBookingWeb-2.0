using NowBookingWeb.Booking.Application.Dto.Output.User;

namespace NowBookingWeb.Booking.Application.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория пользователей.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Метод получает список пользователей для бронирования.
        /// </summary>
        /// <returns>Список пользователей.</returns>
        Task<IEnumerable<UserBookingOutput>> GetUsersBookingsAsync();
    }
}
