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
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/447332112-f0f98d39-7cb9-4f55-971e-462d49aa3a15.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160636Z&X-Amz-Expires=300&X-Amz-Signature=8038c4b054f351bba5bda6ea06d0144cb8bf52f8b2b254fa8468bbe745d72c71&X-Amz-SignedHeaders=host)

## Swagger API Gateway
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/447329360-93db4fd7-8f46-4d01-a0ea-8e36c9ad202f.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160919Z&X-Amz-Expires=300&X-Amz-Signature=0f11e84b409ef893a4f78705f609db39af4b45a221d8572b116bd0076d408e9d&X-Amz-SignedHeaders=host)

## Участники
  * API Gateway - Точка входа
  * CategoryService - Сервис категорий
  * BookingService - Сервис управления `бронированиями`, инициатор `Saga`
  * PaymentService - Сервис управления `платежами`
  * RabbitMQ - Брокер сообщений для `асинхронной` коммуникации
  * gRPC - `Синхронный` вызов между сервисами 

## Диаграмма коммуникации CategoryService (`HTTP/1.1`)
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/447332688-e4ebb220-b094-43b8-aacf-375b09951d57.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160726Z&X-Amz-Expires=300&X-Amz-Signature=aa0e10c60e144fddec01d64b4bc19ec3d23dca081007255c2eebe7a6088d6dca&X-Amz-SignedHeaders=host)

## Диаграмма коммуникации BookingService (`HTTP/2`)
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/447332834-82583cd9-cbab-490a-a0e3-62342cabd853.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160744Z&X-Amz-Expires=300&X-Amz-Signature=7188516869362323a83c20e99ca9b546d041d2a8d5c07fc8cae3eb9e320d229a&X-Amz-SignedHeaders=host)

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
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/445196726-b94ed4c1-7dc7-4552-9fcd-2c215529de83.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160802Z&X-Amz-Expires=300&X-Amz-Signature=396f21c306f7392dea2f12f23ec1a01adc3d9e0e51dd7163162d83af20ae3622&X-Amz-SignedHeaders=host)

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
  
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/445231003-92de311b-7c78-41ce-9bea-2f57530cf9d7.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160820Z&X-Amz-Expires=300&X-Amz-Signature=f86c0080d78d3b8c6ba39a65402976513cef89959dd829d7ad9fe8853bad92e3&X-Amz-SignedHeaders=host)

 Возврат платежа с компенсацией
1. `BookingService` - публикует событие `BookingRefundRequested` в очередь `Booking_Refund_Payment_Request`
2. `PaymentService` - обрабатывает события из очереди, отменяет платеж и публикует результат обработки в очередь `Booking_Refund_Payment_Response`
3. `BookingService` - обрабатывает событие, при обновлении бронирования `падает ошибка`, событие `BookingPaymentRefundResult` публикуется в очередь `Booking_Restore_Payment_Request`
4. `PaymentService` - обрабатывает событие и `восстанавливает платеж`
   
* Диаграмма `с компенсациями` (восстановление платежа при `ошибках бронирования`)
  
![image](https://github-production-user-asset-6210df.s3.amazonaws.com/98191494/445244808-556656ca-4b1b-4841-89cf-a94013cccd81.png?X-Amz-Algorithm=AWS4-HMAC-SHA256&X-Amz-Credential=AKIAVCODYLSA53PQK4ZA%2F20250530%2Fus-east-1%2Fs3%2Faws4_request&X-Amz-Date=20250530T160834Z&X-Amz-Expires=300&X-Amz-Signature=47d754235c0579d031ca827e776df7e142b5e906019e83f14763df2b3cea2f35&X-Amz-SignedHeaders=host)
