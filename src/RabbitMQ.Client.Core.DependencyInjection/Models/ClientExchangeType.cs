namespace RabbitMQ.Client.Core.DependencyInjection.Models
{
    /// <summary>
    /// Custom exchange type that defines which functionality is allowed for an exchange.
    /// </summary>
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