using Microsoft.AspNetCore.Mvc;
using NowBookingWeb.Booking.Application.Dto.Output.Booking;
using NowBookingWeb.Booking.Application.Interfaces.Services;
using NowBookingWeb.Booking.Domain.Enums;
using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.Booking.Api.Controllers
{
    /// <summary>
    /// Контроллер бронирований.
    /// </summary>
    [Route("booking")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly BookingProtoService.BookingProtoServiceClient _bookingServiceGrpcClient;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="bookingService">Сервис бронирований.</param>
        /// <param name="bookingServiceGrpcClient">Клиент сервиса бронирования grpc.</param>
        public BookingController(IBookingService bookingService, 
            BookingProtoService.BookingProtoServiceClient bookingServiceGrpcClient)
        {
            _bookingService = bookingService;
            _bookingServiceGrpcClient = bookingServiceGrpcClient;
        }

        /// <summary>
        /// Метод получает статус бронирования по Id бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Статус бронирования</returns>
        [HttpGet]
        [Route("booking-status")]
        public async Task<IActionResult> GetBookingStatusByBookingIdAsync([FromQuery] int bookingId,
            CancellationToken cancellationToken)
        {
            BookingStatusEnum result = await _bookingService.GetBookingStatusByBookingIdAsync(bookingId,
                cancellationToken);

            return Ok(result.GetDisplayName());
        }

        /// <summary>
        /// Метод обновляет статус бронирования.
        /// </summary>
        /// <param name="bookingId">Id бронирования.</param>
        /// <param name="statusEnum">Статуc бронирования.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <returns>Бронирование с обновленным статусом.</returns>
        [HttpPatch]
        [Route("change-status")]
        public async Task<IActionResult> ChangeBookingStatusAsync([FromQuery] int bookingId,
            [FromQuery] BookingStatusEnum statusEnum, CancellationToken cancellationToken)
        {
            ChangeBookingStatusRequest changeBookingStatusRequest = new()
            {
                BookingId = bookingId,
                StatusEnum = (BookingStatusEnumProto)statusEnum
            };

            ChangeBookingStatusResponse result = await _bookingServiceGrpcClient.ChangeBookingStatusAsync(
                changeBookingStatusRequest);

            return Ok(result.Booking);
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
            BookingOutput result = await _bookingService.RemoveBookingAsync(bookingId, cancellationToken);

            return Ok(result);
        }
    }
}
