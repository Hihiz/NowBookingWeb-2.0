using Grpc.Core;
using NowBookingWeb.Booking.Application.Dto.Output.Booking;
using NowBookingWeb.Booking.Application.Interfaces.Services;
using NowBookingWeb.Booking.Domain.Enums;
using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.Booking.Api.GrpcServices
{
    /// <summary>
    /// Класс реализует методы gRPC сервиса бронирований.
    /// </summary>
    public class BookingGrpcService : BookingProtoService.BookingProtoServiceBase
    {
        private readonly IBookingService _bookingService;

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="bookingService">Сервис бронирований.</param>
        public BookingGrpcService(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        /// <inheritdoc/>
        public async override Task<ChangeBookingStatusResponse> ChangeBookingStatus(ChangeBookingStatusRequest request,
            ServerCallContext context)
        {
            BookingOutput bookingOutput = await _bookingService.ChangeBookingStatusAsync(
                request.BookingId, (BookingStatusEnum)request.StatusEnum);

            ChangeBookingStatusResponse result = new()
            {
                Booking = new Grpc.Contracts.Protos.Booking
                {
                    Id = bookingOutput.Id,
                    StartDate = Timestamp.FromDateTime(bookingOutput.StartDate),
                    EndDate = bookingOutput.EndDate == DateTime.MinValue
                        ? null
                        : Timestamp.FromDateTime(bookingOutput.EndDate),
                    UserId = bookingOutput.UserId,                   
                        ? (BookingStatusEnumProto)bookingOutput.StatusEnum
                        : BookingStatusEnumProto.Undefined,                   
                    CreatedAt = bookingOutput.CreatedAt == DateTime.MinValue
                        ? null
                        : Timestamp.FromDateTime(bookingOutput.CreatedAt)
                }                
            };

            return result;
        }
    }
}
