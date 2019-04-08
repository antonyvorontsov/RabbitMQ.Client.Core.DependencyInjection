namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// RabbitMQ configuration model.
    /// </summary>
    public class RabbitMqClientOptions
    {
        /// <summary>
        /// RabbitMQ server.
        /// </summary>
        public string HostName { get; set; } = "127.0.0.1";
        
        /// <summary>
        /// Port.
        /// </summary>
        public int Port { get; set; } = 5672;
        
        /// <summary>
        /// UserName that connects to the server.
        /// </summary>
        public string UserName { get; set; } = "guest";
        
        /// <summary>
        /// Password of the chosen user.
        /// </summary>
        public string Password { get; set; } = "guest";
        
        /// <summary>
        /// Virtual host.
        /// </summary>
        public string VirtualHost { get; set; } = "/";
    }
}