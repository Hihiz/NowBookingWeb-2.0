namespace NowBookingWeb.Shared.Events.Payment
{
    /// <summary>
    /// Класс модели события запроса возврата средств за бронирование.
    /// </summary>
    public class BookingRefundRequestedEvent
    {
        /// <summary>
        /// Id бронирования.
        /// </summary>
        public int BookingId { get; set; }
    }
}
