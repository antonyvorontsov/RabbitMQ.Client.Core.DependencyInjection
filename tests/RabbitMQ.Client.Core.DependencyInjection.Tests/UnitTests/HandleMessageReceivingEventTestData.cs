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
                    MessageRoutingKey = "file.create",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "file.update",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "file.update",
                    MessageHandlerPatterns = new List<string> { "file.create" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerPatterns = new List<string> { "file.update" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.create", "file.delete" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "file.update",
                    MessageHandlerPatterns = new List<string> { "#" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerPatterns = new List<string> { "*.*.*", "file.delete" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "file.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.#" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "connection.create.stable",
                    MessageHandlerPatterns = new List<string> { "*.*.*" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerPatterns = new List<string> { "#.create.#" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.*", "*.*.*.*", "create.#", "#.create" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.create.*" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "file.update",
                    MessageHandlerPatterns = new List<string> { "*.*.*" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerPatterns = new List<string> { "#.update" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.*.*" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "final.report.create",
                    MessageHandlerPatterns = new List<string> { "#", "*.*.*" },
                    MessageHandlerShouldTrigger = true,
                    AsyncMessageHandlerPatterns = new List<string> { "*.*" },
                    AsyncMessageHandlerShouldTrigger = false,
                    NonCyclicMessageHandlerPatterns = new List<string> { "*.update" },
                    NonCyclicMessageHandlerShouldTrigger = false,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "*.*.create" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = true
                }
            };
            yield return new object[]
            {
                new HandleMessageReceivingEventTestDataModel
                {
                    MessageRoutingKey = "file.update.author.credentials",
                    MessageHandlerPatterns = new List<string> { "*.*", "*.*.*" },
                    MessageHandlerShouldTrigger = false,
                    AsyncMessageHandlerPatterns = new List<string> { "#" },
                    AsyncMessageHandlerShouldTrigger = true,
                    NonCyclicMessageHandlerPatterns = new List<string> { "file.update.author.credentials" },
                    NonCyclicMessageHandlerShouldTrigger = true,
                    AsyncNonCyclicMessageHandlerPatterns = new List<string> { "file.update.credentials" },
                    AsyncNonCyclicMessageHandlerShouldTrigger = false
                }
            };
        }
    }
}