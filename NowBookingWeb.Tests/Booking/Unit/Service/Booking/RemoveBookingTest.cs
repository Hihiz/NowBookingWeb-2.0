using Moq;

namespace NowBookingWeb.Tests.Booking.Unit.Service.Booking
{
    public class RemoveBookingTest : BaseUnitTest
    {
        [Fact]
        public async Task RemoveBookingAsyncTest()
        {
            // Arrange
            _mockBookingRepository
               .Setup(repo => repo.RemoveBookingAsync(1))
               .ReturnsAsync(baseBookingOutputs.First());

            // Act
            var result = await _bookingService.RemoveBookingAsync(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task RemoveBookingAsyncBookingIdZeroTest()
        {
            // Arrange
            _mockBookingRepository
               .Setup(repo => repo.RemoveBookingAsync(1))
               .ReturnsAsync(baseBookingOutputs.First());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.RemoveBookingAsync(0));
        }
    }
}
