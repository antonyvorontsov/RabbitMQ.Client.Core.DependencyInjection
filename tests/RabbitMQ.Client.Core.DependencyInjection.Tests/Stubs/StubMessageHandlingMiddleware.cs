using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Middlewares;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandlingMiddleware : IMessageHandlingMiddleware
    {
        public int Number { get; }

        private readonly Dictionary<int, int> _orderingMap;
        private readonly Dictionary<int, int> _errorOrderingMap;

        public StubMessageHandlingMiddleware(int messageHandlerNumber, Dictionary<int, int> orderingMap, Dictionary<int, int> errorOrderingMap)
        {
            if (!orderingMap.ContainsKey(messageHandlerNumber))
            {
                orderingMap.Add(messageHandlerNumber, 0);
            }
            
            if (!errorOrderingMap.ContainsKey(messageHandlerNumber))
            {
                errorOrderingMap.Add(messageHandlerNumber, 0);
            }

            _orderingMap = orderingMap;
            _errorOrderingMap = errorOrderingMap;
            
            Number = messageHandlerNumber;
        }

        public async Task Handle(MessageHandlingContext context, Func<Task> next)
        {
            var order = _orderingMap.Values.Max();
            _orderingMap[Number] = order + 1;
            await next();
        }

        public async Task HandleError(MessageHandlingContext context, Exception exception, Func<Task> next)
        {
            var order = _errorOrderingMap.Values.Max();
            _errorOrderingMap[Number] = order + 1;
            await next();
        }
    }
}