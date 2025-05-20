using Moq;

namespace NowBookingWeb.Tests.Payment.Unit.Service.Payment
{
    public class GetPaymentByBookingIdTest : BaseUnitTest
    {
        [Fact]
        public async Task GetPaymentByBookingIdAsyncTest()
        {
            // Arrange            
            MockPaymentRepository
                 .Setup(repo => repo.GetPaymentByBookingIdAsync(1))
                 .ReturnsAsync(BasePaymentOutputs.First());

            // Act
            var result = await PaymentService.GetPaymentByBookingIdAsync(1);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public async Task GetPaymentByBookingIdAsyncBookingIdZeroTest()
        {
            // Arrange           
            MockPaymentRepository
                 .Setup(repo => repo.GetPaymentByBookingIdAsync(0))
                 .ThrowsAsync(new InvalidOperationException("Exception"));

            // Act && Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                PaymentService.GetPaymentByBookingIdAsync(0));
        }
    }
}
