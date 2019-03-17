namespace RabbitMQ.Client.Core.Configuration
{
    /// <summary>
    /// Модель опций (конфигурации) RabbitMQ.
    /// </summary>
    public class RabbitMqClientOptions
    {
        /// <summary>
        /// Адрес сервера.
        /// </summary>
        public string HostName { get; set; } = "127.0.0.1";
        
        /// <summary>
        /// Порт.
        /// </summary>
        public int Port { get; set; } = 5672;
        
        /// <summary>
        /// Пользователь.
        /// </summary>
        public string UserName { get; set; } = "guest";
        
        /// <summary>
        /// Пароль.
        /// </summary>
        public string Password { get; set; } = "guest";
        
        /// <summary>
        /// Виртуальный хост (к которому пользователь имеет доступ).
        /// </summary>
        public string VirtualHost { get; set; } = "/";
    }
}