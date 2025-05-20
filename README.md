# NowBookingWeb-2.0

## Стек
* ASP.NET Core 9.0 - Web Api
  *   NowBookingWeb.Booking - сервис бронирования 
  *   NowBookingWeb.Payment - сервис управления платежами
* Angular - Client
* Dapper
* EF Core Migrations
* PostgreSql
* xUnit - Test
* CI
* gRPC
* RabbitMQ

## Swagger
![image](https://github.com/user-attachments/assets/7555140a-ce5a-4d82-a8ab-b02c17b920fa)

## Участники
  * BookingService - Сервис управления бронированиями, инициатор `Saga`
  * PaymentService - Сервис управления платежами
  * RabbitMQ - Брокер сообщений для `асинхронной` коммуникации
  * gRPC - `Синхронный` вызов между сервисами 

## Типы коммуникаций
### 1. Синхронная (gRPC)
**Saga Orchestrator**

**Описание**
- `BookingService` - выступает `оркестратором`. Последовательно вызывает методы, коммуницирует через gRPC и управляет компенсационными действиями при ошибках.

- **Пример работы:**
1. Создает бронирование
2. Вызывает сервис платежей через gRPC для обработки платежа
3. Изменяет статус бронированию
 
- На диаграмме описаны три кейса с компенсациями: `успешное создание`, `при обработке платежа упала ошибка`, `при обновлении статуса упала ошибка`  
![image](https://github.com/user-attachments/assets/b94ed4c1-7dc7-4552-9fcd-2c215529de83)

### 2. Асинхронная (RabbitMQ)
**Saga Choreography**

**Описание**
- Сервисы коммуницируют через `события` используя очередь (брокер RabbitMQ)

- **Пример работы**
- 
  Возврат платежа
1. `BookingService` - публикует событие `BookingRefundRequested` в очередь `Booking_Refund_Payment_Request`
2. `PaymentService` - обрабатывает события из очереди, отменяет платеж и публикует результат обработки в очередь `Booking_Refund_Payment_Response`
3. `BookingService` - обрабатывает событие и `обновляет статус бронированию`

- На диаграммах описаны шаги выполнения `возврата платежа`
* Диаграмма `без компенсаций`
  
![image](https://github.com/user-attachments/assets/92de311b-7c78-41ce-9bea-2f57530cf9d7)

 Возврат платежа с компенсацией
1. `BookingService` - публикует событие `BookingRefundRequested` в очередь `Booking_Refund_Payment_Request`
2. `PaymentService` - обрабатывает события из очереди, отменяет платеж и публикует результат обработки в очередь `Booking_Refund_Payment_Response`
3. `BookingService` - обрабатывает событие, при обновлении бронирования `падает ошибка`, событие `BookingPaymentRefundResult` публикуется в очередь `Booking_Restore_Payment_Request`
4. `PaymentService` - обрабатывает событие и `восстанавливает платеж`
   
* Диаграмма `с компенсациями` (восстановление платежа при `ошибках бронирования`)
  
![image](https://github.com/user-attachments/assets/556656ca-4b1b-4841-89cf-a94013cccd81)
