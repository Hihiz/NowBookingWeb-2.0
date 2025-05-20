namespace NowBookingWeb.Tests.Payment.Integration.Repository.Payment
{
    public class CreatePaymentTest : BaseIntegrationTest
    {
        [Fact]
        public async Task CreatePaymentAsyncTest()
        {
            var result = await PaymentRepository.CreatePaymentAsync(1, Guid.NewGuid().ToString(),
                Grpc.Contracts.Protos.PaymentMethodProtoEnum.Card, Grpc.Contracts.Protos.PaymentCurrentProtoEnum.Usd);

            Assert.True(result > 0);
        }       
    }
}
