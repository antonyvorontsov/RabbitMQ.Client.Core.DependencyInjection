namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    public class RabbitMqConnectionOptions
    {
        public RabbitMqClientOptions ProducerOptions { get; set; }
        
        public RabbitMqClientOptions ConsumerOptions { get; set; }
    }
}