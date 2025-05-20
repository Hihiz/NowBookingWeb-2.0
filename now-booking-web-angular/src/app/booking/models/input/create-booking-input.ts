/**
 * Класс входной модели создания бронирования.
 */
export class CreateBookingInput {
  /**
   * Дата начала бронирования.
   */
  startDate: Date = new Date();

  /**
   * Дата окончания бронирования.
   */
  endDate?: Date | null = null;

  /**
   * Id пользователя, который бронирует.
   */
  userId: number = 0;

  /**
   * Id обьекта бронирования.
   */
  categoryId: number = 0;
}
