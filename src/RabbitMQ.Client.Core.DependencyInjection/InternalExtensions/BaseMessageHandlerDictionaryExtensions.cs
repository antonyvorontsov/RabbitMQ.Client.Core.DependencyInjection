using System.Collections.Generic;
using System.Linq;
using RabbitMQ.Client.Core.DependencyInjection.MessageHandlers;

namespace RabbitMQ.Client.Core.DependencyInjection.InternalExtensions
{
    /// <summary>
    /// An extension class which contains methods for easy operating with dictionaries.
    /// </summary>
    internal static class BaseMessageHandlerDictionaryExtensions
    {
        /// <summary>
        /// Merge (union) keys and values of two dictionaries of specified type.
        /// </summary>
        /// <param name="source">Source dictionary.</param>
        /// <param name="addition">Dictionary which are being merged with the source dictionary.</param>
        /// <returns></returns>
        internal static IDictionary<string, IList<IBaseMessageHandler>> UnionKeysAndValues(
            this IDictionary<string, IList<IBaseMessageHandler>> source,
            IDictionary<string, IList<IBaseMessageHandler>> addition)
        {
            // Rewrite the statements compatible to .Net Standard 2.0
            foreach (var oneItem in addition)
            {
                if (source.ContainsKey(oneItem.Key))
                {
                    source[oneItem.Key] = source[oneItem.Key].Union(oneItem.Value).ToList();
                }
                else
                {
                    source.Add(oneItem.Key, oneItem.Value);
                }
            }
            return source;
        }
    }
}