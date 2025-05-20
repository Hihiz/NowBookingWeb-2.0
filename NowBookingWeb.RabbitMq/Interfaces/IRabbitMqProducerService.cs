using NowBookingWeb.RabbitMq.Enums;

namespace NowBookingWeb.RabbitMq.Interfaces
{
    /// <summary>
    /// Интерфейс для работы очереди RabbitMq.
    /// </summary>
    public interface IRabbitMqProducerService
    {
        /// <summary>
        /// Метод отправляет сообщение в очередь RabbitMq.
        /// </summary>
        /// <typeparam name="T">Тип.</typeparam>
        /// <param name="queueNameEnum">Название типа очереди.</param>
        /// <param name="message">Обьект.</param>
        Task PublishAsync<T>(QueueNameEnum queueNameEnum, T message);
    }
}
