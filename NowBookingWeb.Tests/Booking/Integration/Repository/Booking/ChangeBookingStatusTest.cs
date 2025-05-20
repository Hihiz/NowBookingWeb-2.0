namespace NowBookingWeb.Tests.Booking.Integration.Repository.Booking
{
    public class ChangeBookingStatusTest : BaseIntegrationTest
    {
        [Fact]
        public async Task ChangeBookingStatusAsyncTest()
        {
            var result = await BookingRepository.ChangeBookingStatusAsync(54,
                NowBookingWeb.Booking.Domain.Enums.BookingStatusEnum.Confirmed);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ChangeBookingStatusAsyncExceptionTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                BookingRepository.ChangeBookingStatusAsync(0, NowBookingWeb.Booking.Domain.Enums.BookingStatusEnum
                .Cancelled));
        }
    }
}
