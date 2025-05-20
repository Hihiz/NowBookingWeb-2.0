using NowBookingWeb.Payment.Application.Dto.Output.Payment;

namespace NowBookingWeb.Payment.Application.Interfaces.Repositories
{
    /// <summary>
    /// Интерфейс репозитория платежей.
    /// </summary>
    public interface IPaymentRepository
    {
        /// <summary>
        /// Метод восстанавливает платеж.
        /// </summary>
        /// <param name="transactionId">Id транзакции.</param>
        /// <param name="paymentMethodName">Способ оплаты.</param>
        /// <param name="paymentCurrentName">Валюта.</param>
        /// <param name="bookingId">Id бронирования, для оплаты.</param>
        /// <returns>Выходную модель.</returns>
        Task<PaymentOutput> RestorePaymentAsync(string? transactionId, string? paymentMethodName,
            string? paymentCurrentName, int bookingId);

        /// <summary>
        /// Метод проверяет существование платежа по Id бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <returns>Признак.</returns>
        Task<bool> CheckPaymentByBookingIdAsync(int bookingId);
    }
}
