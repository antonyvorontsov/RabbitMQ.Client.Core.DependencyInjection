using System;
using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services
{
    public class MessageHandlingMock : IMessageHandlingService
    {
        public Task HandleMessageReceivingEvent(BasicDeliverEventArgs eventArgs, IQueueService queueService)
        {
            throw new NotImplementedException();
        }
    }
}
