using System.Threading.Tasks;
using RabbitMQ.Client.Core.DependencyInjection.Models;

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
        /// <param name="context">Model that contains consumed message alongside with additional actions <see cref="MessageHandlingContext"/>.</param>
        Task Execute(MessageHandlingContext context);
    }
}