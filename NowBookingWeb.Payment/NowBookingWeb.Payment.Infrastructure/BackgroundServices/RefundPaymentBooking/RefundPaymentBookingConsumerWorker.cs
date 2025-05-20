using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NowBookingWeb.Payment.Application.Dto.Output.Payment;
using NowBookingWeb.Payment.Application.Interfaces.Services;
using NowBookingWeb.RabbitMq.Enums;
using NowBookingWeb.RabbitMq.Interfaces;
using NowBookingWeb.RabbitMq.Options;
using NowBookingWeb.Shared.Events.Payment;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NowBookingWeb.Payment.Infrastructure.BackgroundServices.RefundPaymentBooking
{
    /// <summary>
    /// Класс воркера обработки сообщений, возврата средств за бронирование.
    /// </summary>
    public class RefundPaymentBookingConsumerWorker : BackgroundService
    {
        private readonly ILogger<RefundPaymentBookingConsumerWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly RabbitMqOptions _rabbitMqOptions;
        private IConnection _connection;
        private IChannel _channel;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="serviceProvider">Сервис провайдер.</param>
        /// <param name="options">Конфигурация RabbitMq.</param>
        public RefundPaymentBookingConsumerWorker(ILogger<RefundPaymentBookingConsumerWorker> logger,
             IServiceProvider serviceProvider,
             IOptions<RabbitMqOptions> options)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _rabbitMqOptions = options.Value;
        }

        /// <summary>
        /// Метод выполняет логику воркера.
        /// </summary>
        /// <param name="stoppingToken">Токен отмены.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            try
            {
                ConnectionFactory factory = new ConnectionFactory()
                {
                    HostName = _rabbitMqOptions.HostName!,
                    UserName = _rabbitMqOptions.UserName!,
                    Password = _rabbitMqOptions.Password!,
                    VirtualHost = _rabbitMqOptions.VirtualHost!
                };

                _connection = await factory.CreateConnectionAsync();
                _channel = await _connection.CreateChannelAsync();

                // Создаем очередь.
                await _channel.QueueDeclareAsync(queue: QueueNameEnum.Booking_Refund_Payment_Request.ToString(),
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                await HandleQueueMessagesAsync();
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }

            // Удерживаем очередь.
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        /// <summary>
        /// Метод обрабатывает сообщения в очереди.
        /// </summary>      
        private async Task HandleQueueMessagesAsync()
        {
            AsyncEventingBasicConsumer consumer = new AsyncEventingBasicConsumer(_channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    using IServiceScope scope = _serviceProvider.CreateScope();

                    IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                    if (ea.Body.IsEmpty)
                    {
                        _logger.LogWarning("Получено пустое сообщение");

                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        return;
                    }

                    byte[] body = ea.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    // Получаем из сообщения, обьект.
                    BookingRefundRequestedEvent? refundPayment = JsonSerializer
                        .Deserialize<BookingRefundRequestedEvent>(message);

                    if (refundPayment is null)
                    {
                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        throw new InvalidOperationException("Ошибка получения события из очереди. " +
                                                            $"Message: {message}.");
                    }

                    // Проверяем наличие платежа.
                    PaymentOutput payment = await paymentService.GetPaymentByBookingIdAsync(refundPayment.BookingId);

                    // Если платежа нет.
                    if (payment is null)
                    {
                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        throw new InvalidOperationException("Ошибка получения платежа для возврата средств. " +
                                                            $"PaymentId: {payment!.Id}. " +
                                                            $"BookingId: {payment.BookingId}.");
                    }

                    _logger.LogInformation("Получен запрос на возврат средств за бронирование. " +
                                           $"PaymentId: {payment.Id}. " +
                                           $"BookingId: {payment.BookingId}");

                    bool isRefunded = await ExecuteRefundPaymentAsync(paymentService, payment.Id,
                        payment.TransactionId!, ea);

                    // Читаем сообщение, что-бы выбросить его из очереди.
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                    // Событие компенсационного действия, восстановления платежа.  
                    PaymentRestoredEvent paymentRestored = new()
                    {
                        Id = payment.Id,
                        TransactionId = payment.TransactionId,
                        PaymentMethodName = payment.PaymentMethod.ToString(),
                        PaymentCurrentName = payment.PaymentCurrent.ToString(),
                        BookingId = payment.BookingId,
                    };

                    BookingPaymentRefundResultEvent result = new()
                    {
                        PaymentId = payment.Id,
                        BookingId = payment.BookingId,
                        IsSuccess = isRefunded,
                        PaymentRestoredEvent = paymentRestored
                    };

                    await PublishResultAsync(result, scope);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            };

            // Подписываемся на очередь.
            await _channel.BasicConsumeAsync(queue: QueueNameEnum.Booking_Refund_Payment_Request.ToString(),
                                            autoAck: false,
                                            consumer: consumer);
        }

        /// <summary>
        /// Переопределенный метод освобождает ресурсы.
        /// </summary>
        public override void Dispose()
        {
            _connection.Dispose();
            _channel.Dispose();

            base.Dispose();
        }
    }
}
