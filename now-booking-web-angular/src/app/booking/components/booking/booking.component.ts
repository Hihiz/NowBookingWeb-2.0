import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

/**
 * Класс компонента бронирований.
 */
@Component({
  selector: 'app-booking',
  standalone: true,
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css',
})
export class BookingComponent implements OnInit {
  public readonly bookings$ = new BehaviorSubject<any>(null);

  /**
   * Конструктор.
   * @param _router Роутер.
   * @param _bookingService Сервис бронирования.
   */
  constructor(
    private readonly _router: Router,
    private readonly _bookingService: BookingService,
  ) {
    this.bookings$ = this._bookingService.bookings$;
  }

  /**
   * Функция получает список бронирования.
   */
  private async getBookingsAsync() {
    (await this._bookingService.getBookingsAsync()).subscribe((_) => {
      console.log('Список бронирований: ', this.bookings$.value);
    );
  }

  /**
   * Функция переходит на страницу создания бронирования.
   */
  public onCreateBooking() {
    this._router.navigate(['/create-booking']);
  }

  /**
   * Функция переходит на страницу редактирования бронирования.
   * @param selectedBookingId Выбранное бронирование.
   */
  public onUpdateBooking(selectedBookingId: number | null) {
    let bookingId;

    bookingId = selectedBookingId;

    this._router.navigate(['/update-booking'], {
      queryParams: {
        bookingId,
      },
    });
  }

  /**
   * Функция оставляет заявку на возврат средств.
   * @param bookingId Id бронирования.
   */
  public async onRefundPaymentBookingAsync(bookingId: number) {
    (await this._bookingService.refundPaymentBookingAsync(bookingId)).subscribe(
      async (_) => {
        console.log('Заявка оставлена');
        await this.getBookingsAsync();
      }
    );
  }

  /**
   * Функция удаляет бронирование.
   * @param selectedBookingId Id бронирования.
   */
  public async onRemoveBookingAsync(selectedBookingId: number) {
    (
      await this._bookingService.removeBookingAsync(selectedBookingId)
    ).subscribe(async (data: any) => {
      console.log('Бронирование удалено: ', data);

      // Получаем актуальный список бронирования.
      await this.getBookingsAsync();
    });
  }

  /**
   * Функция переходит на главную страницу (список с категориями).
   */
  public onGetCategories() {
    this._router.navigate(['/']);
  }
}
