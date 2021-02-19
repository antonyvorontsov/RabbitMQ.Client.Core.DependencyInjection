using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class MessageHandlerDependencyInjectionExtensionsTests
    {
        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameMessageHandlerTwiceForOneRoutingKeyWithDifferentOrder()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                {
                    new ServiceCollection()
                        .AddMessageHandlerTransient<StubMessageHandler>("routing.key", 0)
                        .AddMessageHandlerTransient<StubMessageHandler>("routing.key", 1);
                    return Task.CompletedTask;
                });
        }

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameAsyncMessageHandlerTwiceForOneRoutingKeyWithDifferentOrder()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                new ServiceCollection()
                    .AddAsyncMessageHandlerTransient<StubAsyncMessageHandler>("routing.key", 0)
                    .AddAsyncMessageHandlerTransient<StubAsyncMessageHandler>("routing.key", 1);
                return Task.CompletedTask;
            });
        }
    }
}