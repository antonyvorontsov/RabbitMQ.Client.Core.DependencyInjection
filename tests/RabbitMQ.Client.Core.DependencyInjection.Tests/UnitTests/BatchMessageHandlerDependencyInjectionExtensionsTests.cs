using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;
using RabbitMQ.Client.Core.DependencyInjection.Exceptions;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class BatchMessageHandlerDependencyInjectionExtensionsTests
    {
        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameBatchMessageHandlerTwiceWithConfiguration()
        {
            await Assert.ThrowsAsync<BatchMessageHandlerAlreadyConfiguredException>(() =>
            {
                var configurationMock = new Mock<IConfiguration>();
                new ServiceCollection()
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(configurationMock.Object)
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(configurationMock.Object);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameBatchMessageHandlerTwiceWithOptions()
        {
            await Assert.ThrowsAsync<BatchMessageHandlerAlreadyConfiguredException>(() =>
            {
                new ServiceCollection()
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(new RabbitMqServiceOptions())
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(new RabbitMqServiceOptions());
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameBatchMessageHandlerTwiceWithConfigurationAndOptions()
        {
            await Assert.ThrowsAsync<BatchMessageHandlerAlreadyConfiguredException>(() =>
            {
                var configurationMock = new Mock<IConfiguration>();
                new ServiceCollection()
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(configurationMock.Object)
                    .AddBatchMessageHandler<StubBaseBatchMessageHandler>(new RabbitMqServiceOptions());
                return Task.CompletedTask;
            });
        }
    }
}