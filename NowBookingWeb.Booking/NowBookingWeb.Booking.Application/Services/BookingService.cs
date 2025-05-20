using Microsoft.Extensions.Logging;
using NowBookingWeb.Booking.Application.Dto.Output.Booking;
using NowBookingWeb.Booking.Domain.Enums;
using NowBookingWeb.Grpc.Contracts.Protos;
using NowBookingWeb.Shared.Events.Payment;

namespace NowBookingWeb.Booking.Application.Services
{
    /// <summary>
    /// Класс реализует методы сервиса бронирований.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly ILogger<BookingService> _logger;
        private readonly PaymentProtoService.PaymentProtoServiceClient _paymentServiceGrpcClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="bookingRepository">Репозиторий бронирования.</param>
        /// <param name="logger">Логгер.</param>
        /// <param name="paymentServiceGrpcClient">Сервис платежей gRPC.</param>
        /// <param name="rabbitMqProducerService">Сервис RabbitMq.</param>
        public BookingService(IBookingRepository bookingRepository,
            ILogger<BookingService> logger,
            PaymentProtoService.PaymentProtoServiceClient paymentServiceGrpcClient)
        {
            _bookingRepository = bookingRepository;
            _paymentServiceGrpcClient = paymentServiceGrpcClient;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<BookingOutput>> GetBookingsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                IEnumerable<BookingOutput> result = await _bookingRepository.GetBookingsAsync();

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<BookingStatusEnum> GetBookingStatusByBookingIdAsync(int bookingId,
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

                BookingStatusEnum result = await _bookingRepository.GetBookingStatusByBookingIdAsync(bookingId);

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        /// <inheritdoc />
        public async Task<BookingOutput> CreateBookingAsync(DateTime startDate, DateTime? endDate, long userId,
            int categoryId, CancellationToken cancellationToken = default)
        {
            BookingOutput result = new BookingOutput();

            try
            {
                if (userId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id пользователя. " +
                                                                                  $"UserId: {userId}.");

                    throw ex;
                }

                if (categoryId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException(
                        "Недопустимый Id обьекта бронирования. " +
                        $"BookingItemId: {categoryId}.");

                    throw ex;
                }

                // 1. Создаем бронирование.
                try
                {
                    result = await _bookingRepository.CreateBookingAsync(startDate, endDate, userId,
                        categoryId);

                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    // Передаем ошибку с метода репозитория.
                    throw;
                }

                PaymentResponse paymentResponse = new PaymentResponse();

                // 2. Создаем платеж.
                try
                {
                    PaymentRequest paymentRequest = new()
                    {
                        // Id созданного бронирования.
                        BookingId = result.Id,
                    };

                    // Вызываем gRPC сервис для обработки платежа.
                    paymentResponse = await _paymentServiceGrpcClient.ProccessPaymentAsync(paymentRequest);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    // Выполняем компенсационное действие.
                    await _bookingRepository.RemoveBookingAsync(result.Id);

                    throw new InvalidOperationException("Ошибка при создании платежа. " +
                                                        $"BookingId: {result.Id}. " +
                                                        $"TransactionId: {paymentResponse.TransactionId}.",
                                                        ex);
                }

                // 3. Обновляет статус бронированию.
                try
                {
                    // Если платеж прошел успешно, то меняем статус на Подтверждено.
                    if (paymentResponse.IsSuccess)
                    {
                        result = await _bookingRepository.ChangeBookingStatusAsync(result.Id,
                            BookingStatusEnum.Confirmed);
                    }

                    // Если платеж не прошел, то меняем статус на Отменено.
                    else
                    {
                        result = await _bookingRepository.ChangeBookingStatusAsync(result.Id,
                            BookingStatusEnum.Cancelled);
                    }
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);

                    // Отменяем бронирование.
                    await _bookingRepository.RemoveBookingAsync(result.Id);

                    // Отменяем платеж.
                    await _paymentServiceGrpcClient.DeletePaymentAsync(new PaymentDeleteRequest()
                    {
                        PaymentId = paymentResponse.PaymentId,
                        TransactionId = paymentResponse.TransactionId,
                    });

                    throw new InvalidOperationException("Ошибка при обработке платежа. " +
                                                        $"BookingId: {result.Id}. " +
                                                        $"TransactionId: {paymentResponse.TransactionId}.",
                                                        ex);
                }

                return result;
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

     
        /// <inheritdoc />
        public async Task RefundPaymentBookingAsync(int bookingId, CancellationToken cancellationToken = default)
        {
            try
            {
                if (bookingId <= 0)
                {
                    InvalidOperationException ex = new InvalidOperationException("Недопустимый Id бронирования. " +
                                                                                 $"BookingId: {bookingId}");
                    throw ex;
                }

                BookingOutput booking = await _bookingRepository.GetBookingByBookingIdAsync(bookingId);

                if (booking is null)
                {
                    InvalidOperationException ex = new InvalidOperationException(
                        "Ошибка получения бронирования для отмены платежа. " +
                        $"BookingId: {bookingId}");

                    throw ex;
                }

                // Если статус у бронирования "Ожидает подтверждения" или уже "Отменено" тогда бросаем ошибку, т.к. 
                // не подходящие статусы.

                switch (booking.StatusEnum)
                {
                    case BookingStatusEnum.Pending:
                        throw new InvalidOperationException("Недопустимый статус бронирования." +
                                                            $"BookingId: {booking.Id}. " +
                                                            $"BookingStatus: {booking.StatusName}.");

                    case BookingStatusEnum.Cancelled:
                        throw new InvalidOperationException(
                            "Бронирование уже отменено, заявка на возврат средств активна." +
                            $"BookingId: {booking.Id}. " +
                            $"BookingStatus: {booking.StatusName}.");
                }

                BookingRefundRequestedEvent bookingRefundEvent = new()
                {
                    BookingId = booking.Id
                };

                // Если статус бронирования "Подтверждено" - оплачен.
                if (booking.StatusEnum is BookingStatusEnum.Confirmed)
                {
                    // Отправляем в очередь.
                    await _rabbitMqProducerService.PublishAsync(QueueNameEnum.Booking_Refund_Payment_Request,
                                                                bookingRefundEvent);
                }
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }
    }
}
