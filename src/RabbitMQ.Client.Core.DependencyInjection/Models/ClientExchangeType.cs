namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    public enum ClientExchangeType
    {
        /// <summary>
        /// Exchange is only for consumption.
        /// </summary>
        Consumption,
        
        /// <summary>
        /// Exchange is only for production.
        /// </summary>
        Production,
        
        /// <summary>
        /// Exchange is for both consumption and production.
        /// </summary>
        Universal
    }
}