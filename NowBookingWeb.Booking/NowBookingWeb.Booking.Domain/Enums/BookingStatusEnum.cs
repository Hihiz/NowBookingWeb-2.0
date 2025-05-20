using System.ComponentModel;

namespace NowBookingWeb.Booking.Domain.Enums
{
    /// <summary>
    /// Перечисление статусов бронирования.
    /// </summary>
    public enum BookingStatusEnum
    {
        /// <summary>
        /// Неизвестный статус.
        /// </summary>
        Undefined = 0,

        /// <summary>
        /// Статус ожидания подтверждения.
        /// </summary>
        [Description("Ожидает подтверждения")]
        Pending = 1,

        /// <summary>
        /// Статус подтвержденного бронирования.
        /// </summary>
        [Description("Подтверждено")]
        Confirmed = 2,

        /// <summary>
        /// Статус отмененного бронирования.
        /// </summary>
        [Description("Отменено")]
        Cancelled = 3
    }
}
