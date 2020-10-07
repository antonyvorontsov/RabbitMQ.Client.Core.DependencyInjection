using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubBatchMessageHandlingFilter : IBatchMessageHandlingFilter
    {
        public int MessageHandlerNumber { get; }
            
        private readonly Dictionary<int, int> _handlerOrderMap;
            
        public StubBatchMessageHandlingFilter(int messageHandlerNumber, Dictionary<int, int> handlerOrderMap)
        {
            if (!handlerOrderMap.ContainsKey(messageHandlerNumber))
            {
                handlerOrderMap.Add(messageHandlerNumber, 0);
            }

            _handlerOrderMap = handlerOrderMap;
            MessageHandlerNumber = messageHandlerNumber;
        }

        public Func<IEnumerable<BasicDeliverEventArgs>, CancellationToken, Task> Execute(Func<IEnumerable<BasicDeliverEventArgs>, CancellationToken, Task> next)
        {
            var order = _handlerOrderMap.Values.Max();
            _handlerOrderMap[MessageHandlerNumber] = order + 1;
            return next;
        }
    }
}