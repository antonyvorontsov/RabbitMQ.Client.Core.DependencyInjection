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
        [InlineData("exchange", ExchangeType.Direct, true)]
        [InlineData("exchange", ExchangeType.Topic, true)]
        [InlineData("exchange", ExchangeType.Fanout, true)]
        [InlineData("exchange", ExchangeType.Headers, true)]
        [InlineData("exchange", "wrong.type", false)]
        [InlineData(null, "wrong.type", true)]
        [InlineData("", "wrong.type", true)]
        [InlineData("", ExchangeType.Direct, true)]
        [InlineData("", ExchangeType.Topic, true)]
        [InlineData("", ExchangeType.Fanout, true)]
        [InlineData("", ExchangeType.Headers, true)]
        public void ShouldProperlyValidateDeadLetterExchangeTypeWhenResendingFailedMessagesIsEnabled(string exchangeName, string type, bool expectedResult)
        {
            var specification = new ValidDeadLetterExchangeTypeSpecification();
            var options = new RabbitMqExchangeOptions
            {
                DeadLetterExchange = exchangeName,
                DeadLetterExchangeType = type
            };
            var result = specification.IsSatisfiedBy(options);
            Assert.Equal(expectedResult, result);
        }
    }
}