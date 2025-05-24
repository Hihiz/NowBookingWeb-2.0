using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.ApiGateway.Api.Controllers
{
    /// <summary>
    /// Контроллер бронирований.
    /// </summary>
    [Route("booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly BookingProtoService.BookingProtoServiceClient _bookingProtoServiceClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="bookingProtoServiceClient">Сервис бронирования.</param>
        public BookingController(BookingProtoService.BookingProtoServiceClient bookingProtoServiceClient)
        {
            _bookingProtoServiceClient = bookingProtoServiceClient;
        }

        /// <summary>
        /// Метод получает список бронирований.
        /// </summary>
        /// <returns>Список бронирований.</returns>
        [HttpGet]
        [Route("bookings")]
        public async Task<IActionResult> GetBookingsAsync()
        {
            BookingsResponse result = await _bookingProtoServiceClient.GetBookingsAsync(new Empty());

            return Ok(result.Bookings);
        }

        /// <summary>
        /// Метод удаляет бронирование.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Удаленное бронирование.</returns>
        [HttpDelete]
        [Route("booking/{bookingId}")]
        public async Task<IActionResult> RemoveBookingAsync([FromRoute] int bookingId,
            CancellationToken cancellationToken)
        {
            RemoveBookingRequest request = new()
            {
                BookingId = bookingId
            };

            BookingResponse result = await _bookingProtoServiceClient.RemoveBookingAsync(request);

            return Ok(result.Booking);
        }
    }
}
