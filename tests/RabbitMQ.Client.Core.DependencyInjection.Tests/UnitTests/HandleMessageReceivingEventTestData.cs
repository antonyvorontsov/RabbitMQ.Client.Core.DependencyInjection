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
                    MessageHandlerOrder = 1,
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerOrder = 3,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "another.exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerOrder = null,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerOrder = 2,
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
                    MessageHandlerOrder = 10,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerOrder = null,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerOrder = 10,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerOrder = null,
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
                    MessageHandlerOrder = 0,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerOrder = 0,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerOrder = 0,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerOrder = 0,
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
                    MessageHandlerOrder = null,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.update" },
                    AsyncMessageHandlerOrder = 10,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.update" },
                    NonCyclicMessageHandlerOrder = 10,
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.delete" },
                    AsyncNonCyclicMessageHandlerOrder = null,
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
                    MessageHandlerOrder = 0,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "file.update" },
                    AsyncMessageHandlerOrder = 0,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.update" },
                    NonCyclicMessageHandlerOrder = 20,
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.delete" },
                    AsyncNonCyclicMessageHandlerOrder = 0,
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
                    MessageHandlerOrder = 15,
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*.*", "file.delete" },
                    AsyncMessageHandlerOrder = 20,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "file.update" },
                    NonCyclicMessageHandlerOrder = 30,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.#" },
                    AsyncNonCyclicMessageHandlerOrder = 45,
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
                    MessageHandlerOrder = 10,
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = null,
                    AsyncMessageHandlerPatterns = new List<string> { "#.create.#" },
                    AsyncMessageHandlerOrder = 5,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = null,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "*.*.*.*", "create.#", "#.create" },
                    NonCyclicMessageHandlerOrder = 0,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = null,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.create.*" },
                    AsyncNonCyclicMessageHandlerOrder = 20,
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
                    MessageHandlerOrder = 20,
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#.create.#" },
                    AsyncMessageHandlerOrder = 10,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "*.*.*.*", "create.#", "#.create" },
                    NonCyclicMessageHandlerOrder = 0,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.create.*" },
                    AsyncNonCyclicMessageHandlerOrder = 0,
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
                    MessageHandlerOrder = 0,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#.update" },
                    AsyncMessageHandlerOrder = 0,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerOrder = 0,
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.*.*" },
                    AsyncNonCyclicMessageHandlerOrder = 0,
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
                    MessageHandlerOrder = 10,
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*" },
                    AsyncMessageHandlerOrder = null,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerOrder = null,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.*.create" },
                    AsyncNonCyclicMessageHandlerOrder = 20,
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
                    MessageHandlerOrder = null,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "another.exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "*.*" },
                    AsyncMessageHandlerOrder = null,
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerExchange = "another.exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerOrder = null,
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerExchange = "another.exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.*.create" },
                    AsyncNonCyclicMessageHandlerOrder = null,
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
                    MessageHandlerOrder = 0,
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerExchange = "exchange",
                    AsyncMessageHandlerPatterns = new List<string> { "#" },
                    AsyncMessageHandlerOrder = 0,
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerExchange = "exchange",
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.update.author.credentials" },
                    NonCyclicMessageHandlerOrder = 10,
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerExchange = "exchange",
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.update.credentials" },
                    AsyncNonCyclicMessageHandlerOrder = 0,
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
        }
    }
}