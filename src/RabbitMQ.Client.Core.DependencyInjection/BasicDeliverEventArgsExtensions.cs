using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Core.DependencyInjection
{
    /// <summary>
    /// BasicDeliverEventArgsExtensions extension that help to work with messages,
    /// </summary>
    public static class BasicDeliverEventArgsExtensions
    {
        /// <summary>
        /// Get message from BasicDeliverEventArgs body.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <returns>Message as a string.</returns>
        public static string GetMessage(this BasicDeliverEventArgs eventArgs)
        {
            eventArgs.EnsureIsNotNull();
            return Encoding.UTF8.GetString(eventArgs.Body.ToArray());
        }

        /// <summary>
        /// Get message payload.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <typeparam name="T">Type of a message body.</typeparam>
        /// <returns>Object of type <see cref="T"/>.</returns>
        public static T GetPayload<T>(this BasicDeliverEventArgs eventArgs)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<T>(messageString);
        }
        
        /// <summary>
        /// Get message payload.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="settings">Serializer settings <see cref="JsonSerializerSettings"/>.</param>
        /// <typeparam name="T">Type of a message body.</typeparam>
        /// <returns>Object of type <see cref="T"/>.</returns>
        public static T GetPayload<T>(this BasicDeliverEventArgs eventArgs, JsonSerializerSettings settings)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<T>(messageString, settings);
        }
        
        /// <summary>
        /// Get message payload.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="converters">A collection of json converters <see cref="JsonConverter"/>.</param>
        /// <typeparam name="T">Type of a message body.</typeparam>
        /// <returns>Object of type <see cref="T"/>.</returns>
        public static T GetPayload<T>(this BasicDeliverEventArgs eventArgs, IEnumerable<JsonConverter> converters)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeObject<T>(messageString, converters.ToArray());
        }
        
        /// <summary>
        /// Get message payload as an anonymous object.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="anonymousTypeObject">An anonymous object base.</param>
        /// <typeparam name="T">Type of an anonymous object.</typeparam>
        /// <returns>Anonymous object.</returns>
        public static T GetAnonymousPayload<T>(this BasicDeliverEventArgs eventArgs, T anonymousTypeObject)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeAnonymousType(messageString, anonymousTypeObject);
        }
        
        /// <summary>
        /// Get message payload as an anonymous object.
        /// </summary>
        /// <param name="eventArgs">Message event args.</param>
        /// <param name="anonymousTypeObject">An anonymous object base.</param>
        /// <param name="settings">Serializer settings <see cref="JsonSerializerSettings"/>.</param>
        /// <typeparam name="T">Type of an anonymous object.</typeparam>
        /// <returns>Anonymous object.</returns>
        public static T GetAnonymousPayload<T>(this BasicDeliverEventArgs eventArgs, T anonymousTypeObject, JsonSerializerSettings settings)
        {
            eventArgs.EnsureIsNotNull();
            var messageString = Encoding.UTF8.GetString(eventArgs.Body.ToArray());
            return JsonConvert.DeserializeAnonymousType(messageString, anonymousTypeObject, settings);
        }
        
        static void EnsureIsNotNull(this BasicDeliverEventArgs eventArgs)
        {
            if (eventArgs is null)
            {
                throw new ArgumentNullException(nameof(eventArgs), "BasicDeliverEventArgs have to be not null to parse a message");
            }
        }
    }
}