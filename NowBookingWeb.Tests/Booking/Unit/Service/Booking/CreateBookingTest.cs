using Moq;
using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.Tests.Booking.Unit.Service.Booking
{
    public class CreateBookingTest : BaseUnitTest
    {
        [Fact]
        public async Task CreateBookingAsyncTest()
        {
            //// Arrange
            //_mockBookingRepository
            //    .Setup(repo => repo.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 1))
            //    .ReturnsAsync(baseBookingOutputs.First());

            //// Act
            //var result = await _bookingService.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 1);

            //// Assert
            //Assert.NotNull(result);
            //Assert.Equal(baseBookingOutputs.First().Id, result.Id);
        }

        [Fact]
        public async Task CreateBookingAsyncUserIdZeroTest()
        {
            // Arrange
            _mockBookingRepository
                .Setup(repo => repo.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 0, 1))
                .ThrowsAsync(new InvalidOperationException("Exception"));

            // Act && Assert            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 0, 1));
        }

        [Fact]
        public async Task CreateBookingAsyncCategoryIdZeroTest()
        {
            // Arrange
            _mockBookingRepository
                .Setup(repo => repo.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 0))
                .ThrowsAsync(new InvalidOperationException("Exception"));

            // Act && Assert            
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _bookingService.CreateBookingAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>(), 1, 0));
        }
    }
}
