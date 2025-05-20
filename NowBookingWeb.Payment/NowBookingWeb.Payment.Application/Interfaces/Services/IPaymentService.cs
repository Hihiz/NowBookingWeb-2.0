using NowBookingWeb.Grpc.Contracts.Protos;
using NowBookingWeb.Payment.Application.Dto.Output.Payment;

namespace NowBookingWeb.Payment.Application.Interfaces.Services
{
    /// <summary>
    /// Интерфейс сервиса платежей.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Метод создает платеж в БД.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="transactionId">Id транзакции.</param>
        /// <param name="paymentMethodEnum">Енам с методами оплаты.</param>
        /// <param name="paymentCurrentEnum">Енам с типами валют.</param>
        /// <returns>Id созданного платежа.</returns>
        Task<long> CreatePaymentAsync(int bookingId, string transactionId, PaymentMethodProtoEnum paymentMethodEnum,
            PaymentCurrentProtoEnum paymentCurrentEnum, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод восстанавливает платеж.
        /// </summary>
        /// <param name="transactionId">Id транзакции.</param>
        /// <param name="paymentMethodName">Способ оплаты.</param>
        /// <param name="paymentCurrentName">Валюта.</param>
        /// <param name="bookingId">Id бронирования, для оплаты.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Выходную модель.</returns>
        Task<PaymentOutput> RestorePaymentAsync(string? transactionId, string? paymentMethodName,
            string? paymentCurrentName, int bookingId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Метод проверяет существование платежа по Id бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Признак.</returns>
        Task<bool> CheckPaymentByBookingIdAsync(int bookingId, CancellationToken cancellationToken = default);
    }
}