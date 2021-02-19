using System.Threading.Tasks;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection.Services.Interfaces
{
    /// <summary>
    /// Service that is responsible for handling received message in the pipeline of filters.
    /// </summary>
    public interface IMessageHandlingPipelineExecutingService
    {
        /// <summary>
        /// Execute message handling pipeline.
        /// </summary>
        /// <param name="eventArgs">Received message.</param>
        /// <param name="consumingService">Consuming service.</param>
        Task Execute(BasicDeliverEventArgs eventArgs, IConsumingService consumingService);
    }
}