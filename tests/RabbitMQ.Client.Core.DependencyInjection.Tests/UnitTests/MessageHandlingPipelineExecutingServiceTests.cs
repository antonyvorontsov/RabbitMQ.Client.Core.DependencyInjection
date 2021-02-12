using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class MessageHandlingPipelineExecutingServiceTests
    {
        [Fact]
        public async Task ShouldProperlyExecutePipelineWithNoFilters()
        {
            var argsMock = new Mock<BasicDeliverEventArgs>();
            var queueServiceMock = new Mock<IQueueService>();
            
            var messageHandlingServiceMock = new Mock<IMessageHandlingService>();

            var service = CreateService(
                messageHandlingServiceMock.Object,
                Enumerable.Empty<IMessageHandlingFilter>(),
                Enumerable.Empty<IMessageHandlingExceptionFilter>());

            await service.Execute(argsMock.Object, queueServiceMock.Object);

            messageHandlingServiceMock.Verify(x => x.HandleMessageReceivingEvent(argsMock.Object, queueServiceMock.Object), Times.Once);
        }

        [Fact]
        public async Task ShouldProperlyExecutePipelineInReverseOrder()
        {
            var argsMock = new Mock<BasicDeliverEventArgs>();
            var queueServiceMock = new Mock<IQueueService>();
            
            var messageHandlingServiceMock = new Mock<IMessageHandlingService>();

            var handlerOrderMap = new Dictionary<int, int>();
            var firstFilter = new StubMessageHandlingFilter(1, handlerOrderMap);
            var secondFilter = new StubMessageHandlingFilter(2, handlerOrderMap);
            var thirdFilter = new StubMessageHandlingFilter(3, handlerOrderMap);
            
            var handlingFilters = new List<IMessageHandlingFilter>
            {
                firstFilter,
                secondFilter,
                thirdFilter
            };

            var service = CreateService(
                messageHandlingServiceMock.Object,
                handlingFilters,
                Enumerable.Empty<IMessageHandlingExceptionFilter>());

            await service.Execute(argsMock.Object, queueServiceMock.Object);
            
            messageHandlingServiceMock.Verify(x => x.HandleMessageReceivingEvent(argsMock.Object, queueServiceMock.Object), Times.Once);
            Assert.Equal(1, handlerOrderMap[thirdFilter.MessageHandlerNumber]);
            Assert.Equal(2, handlerOrderMap[secondFilter.MessageHandlerNumber]);
            Assert.Equal(3, handlerOrderMap[firstFilter.MessageHandlerNumber]);
        }

        [Fact]
        public async Task ShouldProperlyExecuteFailurePipelineInReverseOrderWhenMessageHandlingServiceThrowsException()
        {
            var argsMock = new Mock<BasicDeliverEventArgs>();
            var queueServiceMock = new Mock<IQueueService>();

            var exception = new Exception();
            var messageHandlingServiceMock = new Mock<IMessageHandlingService>();
            messageHandlingServiceMock.Setup(x => x.HandleMessageReceivingEvent(argsMock.Object, queueServiceMock.Object))
                .ThrowsAsync(exception);

            var filterOrderMap = new Dictionary<int, int>();
            var firstFilter = new StubMessageHandlingExceptionFilter(1, filterOrderMap);
            var secondFilter = new StubMessageHandlingExceptionFilter(2, filterOrderMap);
            var thirdFilter = new StubMessageHandlingExceptionFilter(3, filterOrderMap);
            var exceptionFilters = new List<IMessageHandlingExceptionFilter>
            {
                firstFilter,
                secondFilter,
                thirdFilter
            };

            var service = CreateService(
                messageHandlingServiceMock.Object,
                Enumerable.Empty<IMessageHandlingFilter>(),
                exceptionFilters);
            
            await service.Execute(argsMock.Object, queueServiceMock.Object);
            
            messageHandlingServiceMock.Verify(x => x.HandleMessageProcessingFailure(exception, argsMock.Object, queueServiceMock.Object), Times.Once);
            Assert.Equal(1, filterOrderMap[thirdFilter.FilterNumber]);
            Assert.Equal(2, filterOrderMap[secondFilter.FilterNumber]);
            Assert.Equal(3, filterOrderMap[firstFilter.FilterNumber]);
        }

        [Fact]
        public async Task ShouldProperlyExecuteFailurePipelineInReverseOrderWhenMessageHandlingFilterThrowsException()
        {
            var argsMock = new Mock<BasicDeliverEventArgs>();
            var queueServiceMock = new Mock<IQueueService>();
            
            var messageHandlingServiceMock = new Mock<IMessageHandlingService>();

            var exception = new Exception();
            var handlingFilter = new Mock<IMessageHandlingFilter>();
            handlingFilter.Setup(x => x.Execute(It.IsAny<Func<BasicDeliverEventArgs, IQueueService, Task>>()))
                .Throws(exception);
            var handlingFilters = new List<IMessageHandlingFilter>
            {
                handlingFilter.Object
            };
            
            var filterOrderMap = new Dictionary<int, int>();
            var firstFilter = new StubMessageHandlingExceptionFilter(1, filterOrderMap);
            var secondFilter = new StubMessageHandlingExceptionFilter(2, filterOrderMap);
            var thirdFilter = new StubMessageHandlingExceptionFilter(3, filterOrderMap);
            var exceptionFilters = new List<IMessageHandlingExceptionFilter>
            {
                firstFilter,
                secondFilter,
                thirdFilter
            };

            var service = CreateService(
                messageHandlingServiceMock.Object,
                handlingFilters,
                exceptionFilters);
            
            await service.Execute(argsMock.Object, queueServiceMock.Object);
            
            messageHandlingServiceMock.Verify(x => x.HandleMessageProcessingFailure(exception, argsMock.Object, queueServiceMock.Object), Times.Once);
            Assert.Equal(1, filterOrderMap[thirdFilter.FilterNumber]);
            Assert.Equal(2, filterOrderMap[secondFilter.FilterNumber]);
            Assert.Equal(3, filterOrderMap[firstFilter.FilterNumber]);
        }

        static IMessageHandlingPipelineExecutingService CreateService(
            IMessageHandlingService messageHandlingService,
            IEnumerable<IMessageHandlingFilter> handlingFilters,
            IEnumerable<IMessageHandlingExceptionFilter> exceptionFilters)
        {
            return new MessageHandlingPipelineExecutingService(
                messageHandlingService,
                handlingFilters,
                exceptionFilters);
        }
    }
}