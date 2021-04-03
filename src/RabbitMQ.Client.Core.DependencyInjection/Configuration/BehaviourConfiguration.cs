namespace RabbitMQ.Client.Core.DependencyInjection.Configuration
{
    /// <summary>
    /// Custom configuration that are responsible for service behaviours.
    /// </summary>
    public class BehaviourConfiguration
    {
        /// <summary>
        /// Flag if logging has to be disabled.
        /// </summary>
        /// <remarks>
        /// It disables only information/warning/debug logs unlike errors. Errors are still logging properly.
        /// </remarks>
        public bool DisableInternalLogging { get; set; }
    }
}