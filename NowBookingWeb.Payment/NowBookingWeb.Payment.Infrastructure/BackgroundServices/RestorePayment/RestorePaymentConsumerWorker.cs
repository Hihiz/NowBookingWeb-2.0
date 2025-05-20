using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NowBookingWeb.Payment.Application.Dto.Output.Payment;
using NowBookingWeb.Payment.Application.Interfaces.Services;
using NowBookingWeb.RabbitMq.Enums;
using NowBookingWeb.RabbitMq.Options;
using NowBookingWeb.Shared.Events.Payment;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NowBookingWeb.Payment.Infrastructure.BackgroundServices.RestorePayment
{
    /// <summary>
    /// Класс воркера обработки событий компенсационных действий, восстановления платежа.
    /// </summary>
    public class RestorePaymentConsumerWorker : BackgroundService
    {
        private readonly ILogger<RestorePaymentConsumerWorker> _logger;      
        private IChannel _channel;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="serviceProvider">Сервис провайдер.</param>
        /// <param name="options">Конфигурация RabbitMq.</param>
        public RestorePaymentConsumerWorker(ILogger<RestorePaymentConsumerWorker> logger,
            IServiceProvider serviceProvider,
            IOptions<RabbitMqOptions> options)
        {
            _logger = logger;
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
                await _channel.QueueDeclareAsync(queue: QueueNameEnum.Booking_Restore_Payment_Request.ToString(),
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
                IPaymentService paymentService = scope.ServiceProvider.GetRequiredService<IPaymentService>();

                try
                {
                    if (ea.Body.IsEmpty)
                    {
                        _logger.LogWarning("Получено пустое сообщение");

                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        return;
                    }

                    byte[] body = ea.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);

                    BookingPaymentRefundResultEvent? result = JsonSerializer
                        .Deserialize<BookingPaymentRefundResultEvent>(message);

                    if (result is null)
                    {
                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        throw new InvalidOperationException("Ошибка получения события из очереди. " +
                                                            $"Message: {message}.");
                    }

                    // Перед восстановлением, проверяем что платежа не существует.
                    bool payment = await paymentService.CheckPaymentByBookingIdAsync(result.BookingId);

                    if (payment)
                    {
                        _logger.LogWarning("Платеж уже восстановлен. " +
                                           $"BookingId: {result.BookingId}. " +
                                           $"PaymentId: {result.PaymentId}. " +
                                           $"IsSuccess: {result.IsSuccess}.");

                        // Читаем сообщение, что-бы выбросить его из очереди.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        return;
                    }

                    // Восстанавливаем платеж.
                    PaymentOutput restoredPayment = await paymentService.RestorePaymentAsync(
                        result.PaymentRestoredEvent!.TransactionId, result.PaymentRestoredEvent.PaymentMethodName,
                        result.PaymentRestoredEvent.PaymentCurrentName, result.PaymentRestoredEvent.BookingId);

                    // Читаем сообщение, что-бы выбросить его из очереди.
                    await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
                }

                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    throw;
                }
            };

            // Подписываемся на очередь.
            await _channel.BasicConsumeAsync(queue: QueueNameEnum.Booking_Restore_Payment_Request.ToString(),
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
