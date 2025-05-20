namespace NowBookingWeb.Tests.Payment.Integration.Repository.Payment
{
    public class CheckPaymentByBookingIdTest : BaseIntegrationTest
    {
        [Fact]
        public async Task CheckPaymentByBookingIdAsyncTest()
        {
            var result = await PaymentRepository.CheckPaymentByBookingIdAsync(1);

            Assert.True(result);
        }

        [Fact]
        public async Task CheckPaymentByBookingIdAsyncFalseTest()
        {
            var result = await PaymentRepository.CheckPaymentByBookingIdAsync(999);

            Assert.False(result);
        }
    }
}
