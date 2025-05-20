using Moq;

namespace NowBookingWeb.Tests.Booking.Unit.Service.Booking
{
    public class GetBookingsTest : BaseUnitTest
    {
        [Fact]
        public async Task GetBookingsAsyncTest()
        {
            // Arrange
            _mockBookingRepository
                .Setup(repo => repo.GetBookingsAsync())
                .ReturnsAsync(baseBookingOutputs);

            // Act
            var result = await _bookingService.GetBookingsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(baseBookingOutputs, result);
        }
    }
}
