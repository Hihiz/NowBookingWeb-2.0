namespace NowBookingWeb.Tests.Booking.Integration.Repository.Booking
{
    public class GetBookingsTest : BaseIntegrationTest
    {
        [Fact]
        public async Task GetBookingsAsyncTest()
        {
            var result = await BookingRepository.GetBookingsAsync();

            Assert.NotNull(result);
        }
    }
}
