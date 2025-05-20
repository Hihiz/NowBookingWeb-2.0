namespace NowBookingWeb.Payment.Domain.Entities
{
    /// <summary>
    /// Класс сущности платежа.
    /// </summary>
    public class PaymentEntity
    {
        /// <summary>
        /// Id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id бронирования.
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Id выполненой транзакции.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Метод оплаты.
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Валюта.
        /// </summary>
        public string PaymentCurrentName { get; set; }
    }
}
