using Moq;
using NowBookingWeb.Grpc.Contracts.Protos;

namespace NowBookingWeb.Tests.Payment.Unit.Service.Payment
{
    public class RestorePaymentTest : BaseUnitTest
    {
        [Fact]
        public async Task RestorePaymentAsyncTest()
        {
            // Arrange
            string transactionId = Guid.NewGuid().ToString();

            MockPaymentRepository
                 .Setup(repo => repo.RestorePaymentAsync(transactionId, PaymentMethodProtoEnum.Card.ToString(),
                 PaymentCurrentProtoEnum.Rub.ToString(), 1))
                 .ReturnsAsync(BasePaymentOutputs.First());

            // Act
            var result = await PaymentService.RestorePaymentAsync(transactionId,
                PaymentMethodProtoEnum.Card.ToString(), PaymentCurrentProtoEnum.Rub.ToString(), 1);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(0, result.Id);
        }
    }
}
