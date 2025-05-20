namespace NowBookingWeb.Shared.Events.Payment
{
    /// <summary>
    /// Класс модели события компенсационного действия платежа.
    /// </summary>
    public class PaymentRestoredEvent
    {
        /// <summary>
        /// Id платежа.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Id транзакции.
        /// </summary>
        public string? TransactionId { get; set; }

        /// <summary>
        /// Перечисление способов оплаты.
        /// </summary>
        public string? PaymentMethodName { get; set; }

        /// <summary>
        /// Перечисление валют.
        /// </summary>
        public string? PaymentCurrentName { get; set; }

        /// <summary>
        /// Id бронирования.
        /// </summary>
        public int BookingId { get; set; }
    }
}
