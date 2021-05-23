using System;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    public class MessageHandlingContext
    {
        private readonly Action<BasicDeliverEventArgs> _ackAction;
        private bool _alreadyAcknowledged;

        public MessageHandlingContext(BasicDeliverEventArgs message, Action<BasicDeliverEventArgs> ackAction, bool disableAutoAck)
        {
            Message = message;
            _ackAction = ackAction;
            AutoAckEnabled = !disableAutoAck;
        }

        public BasicDeliverEventArgs Message { get; }

        public bool AutoAckEnabled { get; }

        public void AcknowledgeMessage()
        {
            if (_alreadyAcknowledged)
            {
                return;
            }

            _ackAction(Message);
            _alreadyAcknowledged = true;
        }
    }
}