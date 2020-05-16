namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// Ssl option model.
    /// </summary>
    public class RabbitMqSslOption
    {
        /// <summary>
        /// Canonical server name.
        /// </summary>
        public string ServerName { get; set; }
        
        /// <summary>
        /// Path to the certificate.
        /// </summary>
        public string CertificatePath { get; set; }

        /// <summary>
        /// Flag that defines if certificate should be used.
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}