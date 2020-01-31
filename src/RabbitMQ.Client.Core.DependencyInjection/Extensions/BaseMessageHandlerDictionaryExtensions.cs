using System.Collections.Generic;
using System.Linq;

namespace RabbitMQ.Client.Core.DependencyInjection.Extensions
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
            foreach (var (key, value) in addition)
            {
                if (source.ContainsKey(key))
                {
                    source[key] = source[key].Union(value).ToList();
                }
                else
                {
                    source.Add(key, value);
                }
            }
            return source;
        }
    }
}