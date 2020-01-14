using System.Collections;
using System.Collections.Generic;

namespace RabbitMQ.Client.Core.DependencyInjection.Tests.UnitTests
{
    public class HandleMessageReceivingEventTestData : IEnumerable<object[]>
    {
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.create",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "another.exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = null,
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = "another.exchange",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.update" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.delete" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = "another.exchange",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.update" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.delete" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "#" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*.*", "file.delete" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "file.update" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.#" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "connection.create.stable",
                    MessageHandlerExchange =  null,
                    MessageHandlerPatterns = new List<string> { "*.*.*" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "#.create.#" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "*.*.*.*", "create.#", "#.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.create.*" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "connection.create.stable",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "*.*.*" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#.create.#" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "*.*.*.*", "create.#", "#.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.create.*" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "*.*.*" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#.update" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.*.*" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "final.report.create",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "#", "*.*.*" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.*.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "final.report.create",
                    MessageHandlerExchange = "another.exchange",
                    MessageHandlerPatterns = new List<string> { "#", "*.*.*" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "another.exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "another.exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.*.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageExchange = "exchange",
                    MessageRoutingKey = "file.update.author.credentials",
                    MessageHandlerExchange = "exchange",
                    MessageHandlerPatterns = new List<string> { "*.*", "*.*.*" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.update.author.credentials" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.update.credentials" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
        }
    }
}