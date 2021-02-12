using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Filters;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.Stubs
{
    public class StubMessageHandlingExceptionFilter : IMessageHandlingExceptionFilter
    {
        public int FilterNumber { get; }
            
        private readonly Dictionary<int, int> _filterOrderMap;
            
        public StubMessageHandlingExceptionFilter(int messageHandlerNumber, Dictionary<int, int> filterOrderMap)
        {
            if (!filterOrderMap.ContainsKey(messageHandlerNumber))
            {
                filterOrderMap.Add(messageHandlerNumber, 0);
            }

            _filterOrderMap = filterOrderMap;
            FilterNumber = messageHandlerNumber;
        }
        
        public Func<Exception, BasicDeliverEventArgs, IQueueService, Task> Execute(Func<Exception, BasicDeliverEventArgs, IQueueService, Task> next)
        {
            var order = _filterOrderMap.Values.Max();
            _filterOrderMap[FilterNumber] = order + 1;
            return next;
        }
    }
}