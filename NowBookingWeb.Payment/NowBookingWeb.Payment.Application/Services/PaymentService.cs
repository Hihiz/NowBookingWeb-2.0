using Microsoft.Extensions.Logging;
using NowBookingWeb.Payment.Application.Dto.Output.Payment;
using NowBookingWeb.Payment.Application.Interfaces.Repositories;
using NowBookingWeb.Payment.Application.Interfaces.Services;

namespace NowBookingWeb.Payment.Application.Services
{
    /// <summary>
    /// Класс реализует методы сервиса платежей.
    /// </summary>
    public class PaymentService : IPaymentService
    {

        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<PaymentService> _logger;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="paymentRepository">Репозиторий платежей.</param>
        /// <param name="logger">Логгер.</param>
        public PaymentService(IPaymentRepository paymentRepository,
            ILogger<PaymentService> logger)
        {
            _paymentRepository = paymentRepository;
            _logger = logger;
        }

        /// <inheritdoc />
        public async Task<PaymentOutput> RestorePaymentAsync(string? transactionId, string? paymentMethodName,
            string? paymentCurrentName, int bookingId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (bookingId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id бронирования. " +
                                                                                 $"BookingId: {bookingId}.");

                    throw ex;
                }

                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id транзакции. " +
                                                                                 $"TransactionId: {transactionId}.");
                    throw ex;
                }

                if (string.IsNullOrWhiteSpace(paymentMethodName))
                {
                    InvalidOperationException ex = new InvalidOperationException(
                        $"Недопустимый тип метода оплаты. PaymentMethodProtoEnum: {paymentMethodName}.");

                    throw ex;
                }

                if (string.IsNullOrWhiteSpace(paymentCurrentName))
                {
                    InvalidOperationException ex = new InvalidOperationException(
                      $"Недопустимый тип валюты. PaymentCurrentProtoEnum: {paymentCurrentName}.");

                    throw ex;
                }

                PaymentOutput result = await _paymentRepository.RestorePaymentAsync(transactionId, paymentMethodName,
                    paymentCurrentName, bookingId);

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> CheckPaymentByBookingIdAsync(int bookingId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (bookingId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id бронирования. " +
                                                                                 $"BookingId: {bookingId}.");

                    throw ex;
                }

                bool result = await _paymentRepository.CheckPaymentByBookingIdAsync(bookingId);

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<bool> RemovePaymentAsync(long paymentId, string transactionId,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (paymentId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id платежа. " +
                                                                                 $"PaymentId: {paymentId}.");

                    throw ex;
                }

                if (string.IsNullOrWhiteSpace(transactionId))
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id транзакции. " +
                                                                                 $"TransactionId: {transactionId}.");
                    throw ex;
                }

                bool result = await _paymentRepository.RemovePaymentAsync(paymentId, transactionId);

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
