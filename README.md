# NowBookingWeb-2.0

## Стек
* ASP.NET Core 9.0 - Web Api
  *   NowBookingWeb.ApiGateway (порт 5000) - единая точка входа
  *   NowBookingWeb.Category (порт 5001) - сервис категорий
  *   NowBookingWeb.Booking (порт 5002) - сервис бронирования 
  *   NowBookingWeb.Payment (порт 5003) - сервис управления платежами
* Angular - Client
* Dapper
* EF Core Migrations
* PostgreSql
* xUnit - Test
* CI pipeline
* gRPC
* RabbitMQ 

## Диаграмма архитектуры
<img width="893" height="233" alt="image" src="https://github.com/user-attachments/assets/d15e8e95-8704-4cd5-a495-9e11a5b3a047" />

## Swagger API Gateway
<img width="798" height="549" alt="image" src="https://github.com/user-attachments/assets/1f3b0f44-13dc-47d6-ab83-f4acac83d868" />

## Участники
  * API Gateway - Точка входа
  * CategoryService - Сервис категорий
  * BookingService - Сервис управления `бронированиями`, инициатор `Saga`
  * PaymentService - Сервис управления `платежами`
  * RabbitMQ - Брокер сообщений для `асинхронной` коммуникации
  * gRPC - `Синхронный` вызов между сервисами 

## Диаграмма коммуникации CategoryService (`HTTP/1.1`)
<img width="893" height="481" alt="image" src="https://github.com/user-attachments/assets/c139e0be-e3d9-433e-a195-d509ad93b08e" />

## Диаграмма коммуникации BookingService (`HTTP/2`)
<img width="893" height="481" alt="image" src="https://github.com/user-attachments/assets/2c2b7852-59c7-4723-ae24-2a67247087e3" />

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
<img width="595" height="806" alt="image" src="https://github.com/user-attachments/assets/971dfb05-3704-4024-9e9a-cfd9d88160a4" />

### 2. Асинхронная (RabbitMQ)
**Saga Choreography**

**Описание**
- Сервисы коммуницируют через `события` используя очередь (брокер RabbitMQ)

- **Пример работы**
  
Возврат платежа
1. `BookingService` - публикует событие `BookingRefundRequested` в очередь `Booking_Refund_Payment_Request`
2. `PaymentService` - обрабатывает события из очереди, отменяет платеж и публикует результат обработки в очередь `Booking_Refund_Payment_Response`
3. `BookingService` - обрабатывает событие и `обновляет статус бронированию`

- На диаграммах описаны шаги выполнения `возврата платежа`
* Диаграмма `без компенсаций`
<img width="654" height="785" alt="image" src="https://github.com/user-attachments/assets/c42a2936-dc0a-47d1-b80f-47924eb53374" />

Возврат платежа с компенсацией
1. `BookingService` - публикует событие `BookingRefundRequested` в очередь `Booking_Refund_Payment_Request`
2. `PaymentService` - обрабатывает события из очереди, отменяет платеж и публикует результат обработки в очередь `Booking_Refund_Payment_Response`
3. `BookingService` - обрабатывает событие, при обновлении бронирования `падает ошибка`, событие `BookingPaymentRefundResult` публикуется в очередь `Booking_Restore_Payment_Request`
4. `PaymentService` - обрабатывает событие и `восстанавливает платеж`
   
* Диаграмма `с компенсациями` (восстановление платежа при `ошибках бронирования`)
  
<img width="513" height="792" alt="image" src="https://github.com/user-attachments/assets/77155128-be4b-4553-bdae-ac61f3f4e318" />
