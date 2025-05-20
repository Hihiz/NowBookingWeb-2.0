namespace NowBookingWeb.Tests.Booking.Integration.Repository.Booking
{
    public class ChangeUserInBookingTest : BaseIntegrationTest
    {
        [Fact]
        public async Task ChangeUserInBookingAsyncTest()
        {
            var result = await BookingRepository.ChangeUserInBookingAsync(2, 2);

            Assert.NotNull(result);
        }

        [Fact]
        public async Task ChangeUserInBookingAsyncExceptionTest()
        {
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                 BookingRepository.ChangeUserInBookingAsync(999, 2));
        }
    }
}
