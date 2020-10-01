using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests.Models;
using RabbitMQ.Client.Events;
using Xunit;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class MessageHandlingServiceTests
    {
        [Theory]
        [ClassData(typeof(HandleMessageReceivingEventTestData))]
        public async Task ShouldProperlyHandleMessageReceivingEvent(HandleMessageReceivingEventTestDataModel testDataModel)
        {
            var exchanges = new List<RabbitMqExchange>
            {
                new RabbitMqExchange { Name = testDataModel.MessageExchange }
            };

            var callOrder = 0;
            int? messageHandlerOrder = null;
            var messageHandlerMock = new Mock<IMessageHandler>();
            messageHandlerMock.Setup(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    callOrder++;
                    messageHandlerOrder = callOrder;
                });
            var messageHandlers = new[] { messageHandlerMock.Object };

            int? asyncMessageHandlerOrder = null;
            var asyncMessageHandlerMock = new Mock<IAsyncMessageHandler>();
            asyncMessageHandlerMock.Setup(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    callOrder++;
                    asyncMessageHandlerOrder = callOrder;
                });
            var asyncMessageHandlers = new[] { asyncMessageHandlerMock.Object };

            int? nonCyclicMessageHandlerOrder = null;
            var nonCyclicMessageHandlerMock = new Mock<INonCyclicMessageHandler>();
            nonCyclicMessageHandlerMock.Setup(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>(), It.IsAny<IQueueService>()))
                .Callback(() =>
                {
                    callOrder++;
                    nonCyclicMessageHandlerOrder = callOrder;
                });
            var nonCyclicMessageHandlers = new[] { nonCyclicMessageHandlerMock.Object };

            int? asyncNonCyclicMessageHandlerOrder = null;
            var asyncNonCyclicMessageHandlerMock = new Mock<IAsyncNonCyclicMessageHandler>();
            asyncNonCyclicMessageHandlerMock.Setup(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>(), It.IsAny<IQueueService>()))
                .Callback(() =>
                {
                    callOrder++;
                    asyncNonCyclicMessageHandlerOrder = callOrder;
                });
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

            var orderingModels = GetMessageHandlerOrderingModels(
                testDataModel,
                messageHandlerMock.Object.GetType(),
                asyncMessageHandlerMock.Object.GetType(),
                nonCyclicMessageHandlerMock.Object.GetType(),
                asyncNonCyclicMessageHandlerMock.Object.GetType());
            
            var testingOrderingModels = GetTestingOrderingModels(
                testDataModel,
                messageHandlerMock,
                asyncMessageHandlerMock,
                nonCyclicMessageHandlerMock,
                asyncNonCyclicMessageHandlerMock);

            var service = CreateService(
                exchanges,
                routers,
                orderingModels,
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
            await service.HandleMessageReceivingEvent(eventArgs, queueService);

            var messageHandlerTimes = testDataModel.MessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            messageHandlerMock.Verify(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>()), messageHandlerTimes);

            var asyncMessageHandlerTimes = testDataModel.AsyncMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            asyncMessageHandlerMock.Verify(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>()), asyncMessageHandlerTimes);

            var nonCyclicMessageHandlerTimes = testDataModel.NonCyclicMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            nonCyclicMessageHandlerMock.Verify(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>(), It.IsAny<IQueueService>()), nonCyclicMessageHandlerTimes);

            var asyncNonCyclicMessageHandlerTimes = testDataModel.AsyncNonCyclicMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            asyncNonCyclicMessageHandlerMock.Verify(x => x.Handle(It.IsAny<BasicDeliverEventArgs>(), It.IsAny<string>(), It.IsAny<IQueueService>()), asyncNonCyclicMessageHandlerTimes);

            var messageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == messageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(messageHandlerCallOrder, messageHandlerOrder);
            var asyncMessageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == asyncMessageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(asyncMessageHandlerCallOrder, asyncMessageHandlerOrder);
            var nonCyclicMessageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == nonCyclicMessageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(nonCyclicMessageHandlerCallOrder, nonCyclicMessageHandlerOrder);
            var asyncNonCyclicMessageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == asyncNonCyclicMessageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(asyncNonCyclicMessageHandlerCallOrder, asyncNonCyclicMessageHandlerOrder);
        }

        static IQueueService CreateQueueService()
        {
            var channelMock = new Mock<IModel>();
            var queueServiceMock = new Mock<IQueueService>();
            queueServiceMock.Setup(x => x.ConsumingChannel)
                .Returns(channelMock.Object);
            return queueServiceMock.Object;
        }

        static IMessageHandlingService CreateService(
            IEnumerable<RabbitMqExchange> exchanges,
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<MessageHandlerOrderingModel> orderingModels,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers,
            IEnumerable<INonCyclicMessageHandler> nonCyclicMessageHandler,
            IEnumerable<IAsyncNonCyclicMessageHandler> asyncNonCyclicMessageHandlers)
        {
            var messageHandlerContainerBuilder = new MessageHandlerContainerBuilder(
                routers,
                orderingModels,
                messageHandlers,
                asyncMessageHandlers,
                nonCyclicMessageHandler,
                asyncNonCyclicMessageHandlers);
            var loggerMock = new Mock<ILogger<MessageHandlingService>>();
            return new MessageHandlingService(messageHandlerContainerBuilder, exchanges, loggerMock.Object);
        }

        static IEnumerable<MessageHandlerOrderingModel> GetMessageHandlerOrderingModels(
            HandleMessageReceivingEventTestDataModel testDataModel,
            Type messageHandlerType,
            Type asyncMessageHandlerType,
            Type nonCyclicMessageHandlerType,
            Type asyncNonCyclicMessageHandlerType)
        {
            var orderingModels = new List<MessageHandlerOrderingModel>();
            if (testDataModel.MessageHandlerOrder.HasValue)
            {
                orderingModels.Add(new MessageHandlerOrderingModel
                {
                    MessageHandlerType =  messageHandlerType,
                    Exchange = testDataModel.MessageHandlerExchange,
                    RoutePatterns = testDataModel.MessageHandlerPatterns,
                    Order = testDataModel.MessageHandlerOrder.Value
                });
            }
            if (testDataModel.AsyncMessageHandlerOrder.HasValue)
            {
                orderingModels.Add(new MessageHandlerOrderingModel
                {
                    MessageHandlerType =  asyncMessageHandlerType,
                    Exchange = testDataModel.AsyncMessageHandlerExchange,
                    RoutePatterns = testDataModel.AsyncMessageHandlerPatterns,
                    Order = testDataModel.AsyncMessageHandlerOrder.Value
                });
            }
            if (testDataModel.NonCyclicMessageHandlerOrder.HasValue)
            {
                orderingModels.Add(new MessageHandlerOrderingModel
                {
                    MessageHandlerType =  nonCyclicMessageHandlerType,
                    Exchange = testDataModel.NonCyclicMessageHandlerExchange,
                    RoutePatterns = testDataModel.NonCyclicMessageHandlerPatterns,
                    Order = testDataModel.NonCyclicMessageHandlerOrder.Value
                });
            }
            if (testDataModel.AsyncNonCyclicMessageHandlerOrder.HasValue)
            {
                orderingModels.Add(new MessageHandlerOrderingModel
                {
                    MessageHandlerType =  asyncNonCyclicMessageHandlerType,
                    Exchange = testDataModel.AsyncNonCyclicMessageHandlerExchange,
                    RoutePatterns = testDataModel.AsyncNonCyclicMessageHandlerPatterns,
                    Order = testDataModel.AsyncNonCyclicMessageHandlerOrder.Value
                });
            }
            return orderingModels;
        }

        static IEnumerable<MessageHandlerOrderingContainerTestModel> GetTestingOrderingModels(
            HandleMessageReceivingEventTestDataModel testDataModel,
            IMock<IMessageHandler> messageHandlerMock,
            IMock<IAsyncMessageHandler> asyncMessageHandlerMock,
            IMock<INonCyclicMessageHandler> nonCyclicMessageHandlerMock,
            IMock<IAsyncNonCyclicMessageHandler> asyncNonCyclicMessageHandlerMock)
        {
            var collection = new List<MessageHandlerOrderingContainerTestModel>
            {
                new MessageHandlerOrderingContainerTestModel
                {
                    MessageHandler = messageHandlerMock.Object,
                    ShouldTrigger = testDataModel.MessageHandlerShouldTrigger,
                    OrderValue = testDataModel.MessageHandlerOrder
                },
                new MessageHandlerOrderingContainerTestModel
                {
                    MessageHandler = asyncMessageHandlerMock.Object,
                    ShouldTrigger = testDataModel.AsyncMessageHandlerShouldTrigger,
                    OrderValue = testDataModel.AsyncMessageHandlerOrder
                },
                new MessageHandlerOrderingContainerTestModel
                {
                    MessageHandler = nonCyclicMessageHandlerMock.Object,
                    ShouldTrigger = testDataModel.NonCyclicMessageHandlerShouldTrigger,
                    OrderValue = testDataModel.NonCyclicMessageHandlerOrder
                },
                new MessageHandlerOrderingContainerTestModel
                {
                    MessageHandler = asyncNonCyclicMessageHandlerMock.Object,
                    ShouldTrigger = testDataModel.AsyncNonCyclicMessageHandlerShouldTrigger,
                    OrderValue = testDataModel.AsyncNonCyclicMessageHandlerOrder
                }
            };

            var callOrder = 1;
            var orderedCollection = collection.OrderByDescending(x => x.OrderValue)
                .ThenByDescending(x => x.MessageHandler.GetHashCode())
                .ToList();
            foreach (var item in orderedCollection.Where(item => item.ShouldTrigger))
            {
                item.CallOrder = callOrder++;
            }
            return orderedCollection;
        }
    }
}