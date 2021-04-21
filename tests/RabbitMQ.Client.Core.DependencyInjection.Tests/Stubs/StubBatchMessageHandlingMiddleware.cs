using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubBatchMessageHandlingMiddleware : IBatchMessageHandlingMiddleware
    {
        public int Number { get; }

        private readonly Dictionary<int, int> _orderingMap;

        public StubBatchMessageHandlingMiddleware(int messageHandlerNumber, Dictionary<int, int> orderingMap)
        {
            if (!orderingMap.ContainsKey(messageHandlerNumber))
            {
                orderingMap.Add(messageHandlerNumber, 0);
            }

            _orderingMap = orderingMap;
            Number = messageHandlerNumber;
        }
        
        public async Task Handle(IEnumerable<BasicDeliverEventArgs> messages, Func<Task> next, CancellationToken cancellationToken)
        {
            var order = _orderingMap.Values.Max();
            _orderingMap[Number] = order + 1;
            await next();
        }
    }
}