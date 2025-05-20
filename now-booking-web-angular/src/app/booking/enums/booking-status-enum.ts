/**
 * Перечисление статусов бронирования.
 */
export enum BookingStatusEnum {
  /**
   * Неизвестный тип.
   */
  Undefined = 0,

  /**
   * Ожидает подтверждения.
   */
  Pending = 1,

  /**
   * Подтверждено.
   */
  Confirmed = 2,

  /**
   * Отменено.
   */
  Cancelled = 3,
}
