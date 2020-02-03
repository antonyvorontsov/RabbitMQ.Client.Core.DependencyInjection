using System;
using RabbitMQ.Client.Core.DependencyInjection.Configuration;

namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    public class RabbitMqConnectionOptionsContainer
    {
        public Guid Guid { get; set; }
        
        public RabbitMqConnectionOptions Options { get; set; }
    }
}