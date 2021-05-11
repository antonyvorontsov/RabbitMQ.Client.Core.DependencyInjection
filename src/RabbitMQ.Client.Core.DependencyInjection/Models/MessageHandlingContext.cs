using System;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    public class MessageHandlingContext
    {
        public MessageHandlingContext(BasicDeliverEventArgs message, Action<BasicDeliverEventArgs> ackAction, bool disableAutoAck)
        {
            Message = message;
            AckAction = ackAction;
            AutoAckEnabled = !disableAutoAck;
        }

        public BasicDeliverEventArgs Message { get; }
        
        public Action<BasicDeliverEventArgs> AckAction { get; }
        
        public bool AutoAckEnabled { get; }
    }
}