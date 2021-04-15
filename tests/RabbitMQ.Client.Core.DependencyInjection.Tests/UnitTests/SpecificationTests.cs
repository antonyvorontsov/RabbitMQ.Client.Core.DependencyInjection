using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Specifications;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class SpecificationTests
    {
        [Theory]
        [InlineData(ExchangeType.Direct, true)]
        [InlineData(ExchangeType.Topic, true)]
        [InlineData(ExchangeType.Fanout, true)]
        [InlineData(ExchangeType.Headers, true)]
        [InlineData("wrong.type", false)]
        public void ShouldProperlyValidateExchangeType(string type, bool expectedResult)
        {
            var specification = new ValidExchangeTypeSpecification();
            var options = new RabbitMqExchangeOptions
            {
                Type = type
            };
            var result = specification.IsSatisfiedBy(options);
            Assert.Equal(expectedResult, result);
        }
        
        [Theory]
        [InlineData(true, ExchangeType.Direct, true)]
        [InlineData(true, ExchangeType.Topic, true)]
        [InlineData(true, ExchangeType.Fanout, true)]
        [InlineData(true, ExchangeType.Headers, true)]
        [InlineData(true, "wrong.type", false)]
        [InlineData(false, "wrong.type", true)]
        [InlineData(false, ExchangeType.Headers, true)]
        public void ShouldProperlyValidateDeadLetterExchangeTypeWhenResendingFailedMessagesIsEnabled(bool resendingEnabled, string type, bool expectedResult)
        {
            var specification = new ValidDeadLetterExchangeTypeSpecification();
            var options = new RabbitMqExchangeOptions
            {
                RequeueFailedMessages = resendingEnabled,
                DeadLetterExchangeType = type
            };
            var result = specification.IsSatisfiedBy(options);
            Assert.Equal(expectedResult, result);
        }
    }
}