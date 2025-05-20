namespace NowBookingWeb.Shared.Events.Payment
{
    /// <summary>
    /// Класс модели события результата возврата средств за бронирование.
    /// </summary>
    public class BookingPaymentRefundResultEvent
    {
        /// <summary>
        /// Id платежа, который отменили.
        /// </summary>
        public int PaymentId { get; set; }

        /// <summary>
        /// Id бронирования, для которого отменяли платеж (возвращали средства).
        /// </summary>
        public int BookingId { get; set; }

        /// <summary>
        /// Признак возврата средств за бронирование.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Событие восстановления платежа (компенсационное действие).
        /// </summary>
        public PaymentRestoredEvent? PaymentRestoredEvent { get; set; }
    }
}
