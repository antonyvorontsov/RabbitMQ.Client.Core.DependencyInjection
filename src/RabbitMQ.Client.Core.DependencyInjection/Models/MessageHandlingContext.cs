using System;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    public class MessageHandlingContext
    {
        public MessageHandlingContext(BasicDeliverEventArgs message, Action<BasicDeliverEventArgs>? ackAction)
        {
            Message = message;
            AckAction = ackAction;
        }

        public BasicDeliverEventArgs Message { get; }
        
        public Action<BasicDeliverEventArgs>? AckAction { get; }
    }
}