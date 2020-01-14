using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class MessageHandlingServiceTests
    {
        [Theory]
        [ClassData(typeof(HandleMessageReceivingEventTestData))]
        public void ShouldProperlyHandleMessageReceivingEvent(HandleMessageReceivingEventTestDataModel testDataModel)
        {
            var exchanges = new List<RabbitMqExchange>
            {
                new RabbitMqExchange { Name = testDataModel.MessageExchange }
            };

            var messageHandlerMock = new Mock<IMessageHandler>();
            var messageHandlers = new[] { messageHandlerMock.Object };
            
            var asyncMessageHandlerMock = new Mock<IAsyncMessageHandler>();
            var asyncMessageHandlers = new[] { asyncMessageHandlerMock.Object };
            
            var nonCyclicMessageHandlerMock = new Mock<INonCyclicMessageHandler>();
            var nonCyclicMessageHandlers = new[] { nonCyclicMessageHandlerMock.Object };
            
            var asyncNonCyclicMessageHandlerMock = new Mock<IAsyncNonCyclicMessageHandler>();
            var asyncNonCyclicMessageHandlers = new[] { asyncNonCyclicMessageHandlerMock.Object };
            
            var routers = new List<MessageHandlerRouter>
            {
                new MessageHandlerRouter
                {
                    Type = messageHandlerMock.Object.GetType(),
                    Exchange = testDataModel.MessageHandlerExchange,
                    RoutePatterns = testDataModel.MessageHandlerPatterns
                },
                new MessageHandlerRouter
                {
                    Type = asyncMessageHandlerMock.Object.GetType(),
                    Exchange = testDataModel.AsyncMessageHandlerExchange,
                    RoutePatterns = testDataModel.AsyncMessageHandlerPatterns
                },
                new MessageHandlerRouter
                {
                    Type = nonCyclicMessageHandlerMock.Object.GetType(),
                    Exchange = testDataModel.NonCyclicMessageHandlerExchange,
                    RoutePatterns = testDataModel.NonCyclicMessageHandlerPatterns
                },
                new MessageHandlerRouter
                {
                    Type = asyncNonCyclicMessageHandlerMock.Object.GetType(),
                    Exchange = testDataModel.AsyncNonCyclicMessageHandlerExchange,
                    RoutePatterns = testDataModel.AsyncNonCyclicMessageHandlerPatterns
                }
            };
            
            var service = CreateService(
                exchanges,
                routers,
                messageHandlers,
                asyncMessageHandlers,
                nonCyclicMessageHandlers,
                asyncNonCyclicMessageHandlers);
            var queueService = CreateQueueService();
            
            var eventArgs = new BasicDeliverEventArgs
            {
                Exchange = testDataModel.MessageExchange,
                RoutingKey = testDataModel.MessageRoutingKey,
                Body = Array.Empty<byte>()
            };
            service.HandleMessageReceivingEvent(eventArgs, queueService);

            var messageHandlerTimes = testDataModel.MessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            messageHandlerMock.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>()), messageHandlerTimes);
            
            var asyncMessageHandlerTimes = testDataModel.AsyncMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            asyncMessageHandlerMock.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>()), asyncMessageHandlerTimes);
            
            var nonCyclicMessageHandlerTimes = testDataModel.NonCyclicMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            nonCyclicMessageHandlerMock.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IQueueService>()), nonCyclicMessageHandlerTimes);
            
            var asyncNonCyclicMessageHandlerTimes = testDataModel.AsyncNonCyclicMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            asyncNonCyclicMessageHandlerMock.Verify(x => x.Handle(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<IQueueService>()), asyncNonCyclicMessageHandlerTimes);
        }

        static IQueueService CreateQueueService()
        {
            var channelMock = new Mock<IModel>();
            var queueServiceMock = new Mock<IQueueService>();
            queueServiceMock.Setup(x => x.Channel)
                .Returns(channelMock.Object);
            return queueServiceMock.Object;
        }

        static IMessageHandlingService CreateService(
            IEnumerable<RabbitMqExchange> exchanges,
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicMessageHandler,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicMessageHandlers)
        {
            var loggerMock = new Mock<ILogger<MessageHandlingService>>();
            return new MessageHandlingService(
                exchanges,
                routers,
                messageHandlers,
                asyncMessageHandlers,
                nonCyclicMessageHandler,
                asyncNonCyclicMessageHandlers,
                loggerMock.Object);
        }
    }
}