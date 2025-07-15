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
![image](https://github.com/user-attachments/assets/f0f98d39-7cb9-4f55-971e-462d49aa3a15)

## Swagger API Gateway
![image](https://github.com/user-attachments/assets/93db4fd7-8f46-4d01-a0ea-8e36c9ad202f)

## Участники
  * API Gateway - Точка входа
  * CategoryService - Сервис категорий
  * BookingService - Сервис управления `бронированиями`, инициатор `Saga`
  * PaymentService - Сервис управления `платежами`
  * RabbitMQ - Брокер сообщений для `асинхронной` коммуникации
  * gRPC - `Синхронный` вызов между сервисами 

## Диаграмма коммуникации CategoryService (`HTTP/1.1`)
<img width="1104" height="573" alt="image" src="https://github.com/user-attachments/assets/59e8db61-8033-4aeb-a301-986f52f22846" />

## Диаграмма коммуникации BookingService (`HTTP/2`)
![image](https://github.com/user-attachments/assets/82583cd9-cbab-490a-a0e3-62342cabd853)

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
