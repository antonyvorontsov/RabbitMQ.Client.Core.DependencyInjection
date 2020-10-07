using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Core.DependencyInjection.Services;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandlingFilter : IMessageHandlingFilter
    {
        public int MessageHandlerNumber { get; }
            
        private readonly Dictionary<int, int> _handlerOrderMap;
            
        public StubMessageHandlingFilter(int messageHandlerNumber, Dictionary<int, int> handlerOrderMap)
        {
            if (!handlerOrderMap.ContainsKey(messageHandlerNumber))
            {
                handlerOrderMap.Add(messageHandlerNumber, 0);
            }

            _handlerOrderMap = handlerOrderMap;
            MessageHandlerNumber = messageHandlerNumber;
        }

        public Func<BasicDeliverEventArgs, IQueueService, Task> Execute(Func<BasicDeliverEventArgs, IQueueService, Task> next)
        {
            var order = _handlerOrderMap.Values.Max();
            _handlerOrderMap[MessageHandlerNumber] = order + 1;
            return next;
        }
    }
}