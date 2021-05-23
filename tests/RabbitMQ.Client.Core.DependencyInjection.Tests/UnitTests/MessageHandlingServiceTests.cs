using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces;
using RabbitMQ.Client.Core.DependencyInjection.Tests.Models;
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
            var callOrder = 0;
            int? messageHandlerOrder = null;
            var messageHandlerMock = new Mock<IMessageHandler>();
            messageHandlerMock.Setup(x => x.Handle(It.IsAny<MessageHandlingContext>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    callOrder++;
                    messageHandlerOrder = callOrder;
                });
            var messageHandlers = new[] { messageHandlerMock.Object };

            int? asyncMessageHandlerOrder = null;
            var asyncMessageHandlerMock = new Mock<IAsyncMessageHandler>();
            asyncMessageHandlerMock.Setup(x => x.Handle(It.IsAny<MessageHandlingContext>(), It.IsAny<string>()))
                .Callback(() =>
                {
                    callOrder++;
                    asyncMessageHandlerOrder = callOrder;
                });
            var asyncMessageHandlers = new[] { asyncMessageHandlerMock.Object };

            var routers = new List<MessageHandlerRouter>
            {
                new(messageHandlerMock.Object.GetType(), testDataModel.MessageHandlerExchange, testDataModel.MessageHandlerPatterns),
                new(asyncMessageHandlerMock.Object.GetType(), testDataModel.AsyncMessageHandlerExchange, testDataModel.AsyncMessageHandlerPatterns)
            };

            var orderingModels = GetMessageHandlerOrderingModels(
                testDataModel,
                messageHandlerMock.Object.GetType(),
                asyncMessageHandlerMock.Object.GetType());

            var testingOrderingModels = GetTestingOrderingModels(
                    testDataModel,
                    messageHandlerMock,
                    asyncMessageHandlerMock)
                .ToList();

            var service = CreateService(
                routers,
                orderingModels,
                messageHandlers,
                asyncMessageHandlers);
            
            var eventArgs = new BasicDeliverEventArgs
            {
                Exchange = testDataModel.MessageExchange,
                RoutingKey = testDataModel.MessageRoutingKey,
                Body = Array.Empty<byte>()
            };
            var context = new MessageHandlingContext(eventArgs, _ => { }, false);
            await service.HandleMessageReceivingEvent(context);

            var messageHandlerTimes = testDataModel.MessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            messageHandlerMock.Verify(x => x.Handle(It.IsAny<MessageHandlingContext>(), It.IsAny<string>()), messageHandlerTimes);

            var asyncMessageHandlerTimes = testDataModel.AsyncMessageHandlerShouldTrigger ? Times.Once() : Times.Never();
            asyncMessageHandlerMock.Verify(x => x.Handle(It.IsAny<MessageHandlingContext>(), It.IsAny<string>()), asyncMessageHandlerTimes);

            var messageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == messageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(messageHandlerCallOrder, messageHandlerOrder);
            var asyncMessageHandlerCallOrder = testingOrderingModels.FirstOrDefault(x => x.MessageHandler.GetType() == asyncMessageHandlerMock.Object.GetType())?.CallOrder;
            Assert.Equal(asyncMessageHandlerCallOrder, asyncMessageHandlerOrder);
        }

        private static IMessageHandlingService CreateService(
            IEnumerable<MessageHandlerRouter> routers,
            IEnumerable<MessageHandlerOrderingModel> orderingModels,
            IEnumerable<IMessageHandler> messageHandlers,
            IEnumerable<IAsyncMessageHandler> asyncMessageHandlers)
        {
            var messageHandlerContainerBuilder = new MessageHandlerContainerBuilder(
                routers,
                orderingModels,
                messageHandlers,
                asyncMessageHandlers);
            var loggingServiceMock = new Mock<ILoggingService>();
            return new MessageHandlingService(
                messageHandlerContainerBuilder,
                loggingServiceMock.Object);
        }

        private static IEnumerable<MessageHandlerOrderingModel> GetMessageHandlerOrderingModels(
            HandleMessageReceivingEventTestDataModel testDataModel,
            Type messageHandlerType,
            Type asyncMessageHandlerType)
        {
            var orderingModels = new List<MessageHandlerOrderingModel>();
            if (testDataModel.MessageHandlerOrder.HasValue)
            {
                orderingModels.Add(
                    new MessageHandlerOrderingModel(
                        messageHandlerType,
                        testDataModel.MessageHandlerExchange,
                        testDataModel.MessageHandlerPatterns,
                        testDataModel.MessageHandlerOrder.Value));
            }
            if (testDataModel.AsyncMessageHandlerOrder.HasValue)
            {
                orderingModels.Add(
                    new MessageHandlerOrderingModel(
                        asyncMessageHandlerType,
                        testDataModel.AsyncMessageHandlerExchange,
                        testDataModel.AsyncMessageHandlerPatterns,
                        testDataModel.AsyncMessageHandlerOrder.Value));
            }
            return orderingModels;
        }

        private static IEnumerable<MessageHandlerOrderingContainerTestModel> GetTestingOrderingModels(
            HandleMessageReceivingEventTestDataModel testDataModel,
            IMock<IMessageHandler> messageHandlerMock,
            IMock<IAsyncMessageHandler> asyncMessageHandlerMock)
        {
            var collection = new List<MessageHandlerOrderingContainerTestModel>
            {
                new()
                {
                    MessageHandler = messageHandlerMock.Object,
                    ShouldTrigger = testDataModel.MessageHandlerShouldTrigger,
                    OrderValue = testDataModel.MessageHandlerOrder
                },
                new()
                {
                    MessageHandler = asyncMessageHandlerMock.Object,
                    ShouldTrigger = testDataModel.AsyncMessageHandlerShouldTrigger,
                    OrderValue = testDataModel.AsyncMessageHandlerOrder
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