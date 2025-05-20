using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NowBookingWeb.Booking.Application.Interfaces.Services;
using NowBookingWeb.Booking.Domain.Enums;
using NowBookingWeb.RabbitMq.Enums;
using NowBookingWeb.RabbitMq.Interfaces;
using NowBookingWeb.RabbitMq.Options;
using NowBookingWeb.Shared.Events.Payment;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace NowBookingWeb.Booking.Infrastructure.BackgroundServices.RefundPaymentBooking
{
    /// <summary>
    /// Класс воркера обработки сообщений, возврата средств за бронирование.
    /// </summary>
    public class RefundPaymentBookingConsumerWorker : BackgroundService
    {
        private readonly ILogger<RefundPaymentBookingConsumerWorker> _logger;
        private readonly IServiceProvider _serviceProvider;
        private IConnection _connection;
        private IChannel _channel;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="logger">Логгер.</param>
        /// <param name="serviceProvider">Сервис провайдер.</param>
        /// <param name="options">Конфигурация RabbitMq.</param>
        public RefundPaymentBookingConsumerWorker(ILogger<RefundPaymentBookingConsumerWorker> logger,
             IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                await _channel.QueueDeclareAsync(queue: QueueNameEnum.Booking_Refund_Payment_Response.ToString(),
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
                using IServiceScope scope = _serviceProvider.CreateScope();

                IRabbitMqProducerService rabbitMqService = scope.ServiceProvider
                    .GetRequiredService<IRabbitMqProducerService>();

                BookingPaymentRefundResultEvent? result = null;

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

                    result = JsonSerializer.Deserialize<BookingPaymentRefundResultEvent>(message);

                    if (result is null)
                    {
                        // Чистим очередь от пустых сообщений.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        throw new InvalidOperationException("Ошибка получения события из очереди. " +
                                                            $"Message: {message}.");
                    }

                    try
                    {
                        // Меняем статус бронирования на "Отменен".
                        await bookingService.ChangeBookingStatusAsync(result.BookingId, BookingStatusEnum.Cancelled);
                    }

                    catch (Exception ex)
                    {
                        _logger.LogError("Ошибка изменения статуса бронирования." +
                                         $"BookingId: {result.BookingId}.");

                        // Компенсационное действие для платежа.
                        await rabbitMqService.PublishAsync(QueueNameEnum.Booking_Restore_Payment_Request, result);

                        // Читаем сообщение, что-бы выбросить его из очереди.
                        await _channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);

                        return;
                    }

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
            await _channel.BasicConsumeAsync(queue: QueueNameEnum.Booking_Refund_Payment_Response.ToString(),
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