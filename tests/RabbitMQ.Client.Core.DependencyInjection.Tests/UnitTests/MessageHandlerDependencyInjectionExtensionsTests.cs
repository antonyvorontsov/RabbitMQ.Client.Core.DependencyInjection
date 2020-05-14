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

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameNonCyclicMessageHandlerTwiceForOneRoutingKeyWithDifferentOrder()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                new ServiceCollection()
                    .AddNonCyclicMessageHandlerTransient<StubNonCyclicMessageHandler>("routing.key", 0)
                    .AddNonCyclicMessageHandlerTransient<StubNonCyclicMessageHandler>("routing.key", 1);
                return Task.CompletedTask;
            });
        }

        [Fact]
        public async Task ShouldProperlyThrowExceptionWhenRegisteringSameAsyncNonCyclicMessageHandlerTwiceForOneRoutingKeyWithDifferentOrder()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
            {
                new ServiceCollection()
                    .AddAsyncNonCyclicMessageHandlerTransient<StubAsyncNonCyclicMessageHandler>("routing.key", 0)
                    .AddAsyncNonCyclicMessageHandlerTransient<StubAsyncNonCyclicMessageHandler>("routing.key", 1);
                return Task.CompletedTask;
            });
        }
    }
}