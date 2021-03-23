using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;
using RabbitMQ.Client.Core.DependencyInjection.Models;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// Internal extensions for easier work with MessageHandlerContainer model.
    /// </summary>
    internal static class MessageHandlerContainerExtensions
    {
        internal static int? GetOrderForHandler(this MessageHandlerContainer container, IBaseMessageHandler handler)
        {
            return container.MessageHandlerOrderingModels.FirstOrDefault(x => x.MessageHandlerType == handler.GetType())?.Order;
        }
    }
}