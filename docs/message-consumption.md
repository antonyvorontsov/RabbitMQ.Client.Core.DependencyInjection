# Message consumption

### Starting a consumer

The first step that has to be done to retrieve messages from queues is to start a consumer. This can be achieved by calling the `StartConsuming` method of `IQueueService`.
Consumption exchanges will work only in a message-production mode if the `StartConsuming` method won't be called.

Let's say that your configuration looks like this.

```c#
public class Startup
{
    public static IConfiguration Configuration;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var clientConfiguration = Configuration.GetSection("RabbitMq");
        var exchangeConfiguration = Configuration.GetSection("RabbitMqExchange");
        services.AddRabbitMqClient(clientConfiguration)
            .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration);
    }
}
```

You can register `IHostedService` and inject an instance of `IQueueService` into it.

```c#
services.AddSingleton<IHostedService, ConsumingService>();
```

And then simply call `StartConsuming` so a consumer can work in the background.

```c#
public class ConsumingService : IHostedService
{
    readonly IQueueService _queueService;
    readonly ILogger<ConsumingService> _logger;

    public ConsumingService(
        IQueueService queueService,
        ILogger<ConsumingService> logger)
    {
        _queueService = queueService;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting consuming.");
        _queueService.StartConsuming();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping consuming.");
        return Task.CompletedTask;
    }
}
```

Otherwise, you can implement a worker service template from .Net Core 3 like this.

```c#
public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                var clientConfiguration = hostContext.Configuration.GetSection("RabbitMq");
                var exchangeConfiguration = hostContext.Configuration.GetSection("RabbitMqExchange");
                services.AddRabbitMqClient(clientConfiguration)
                    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration);

                // And add the background service.
                services.AddHostedService<Worker>();;
            });
}

public class Worker : BackgroundService
{
    private readonly IQueueService _queueService;

    public Worker(IQueueService queueService)
    {
        _queueService = queueService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _queueService.StartConsuming();
    }
}
```

The second step is to define classes that will take responsibility of handling received messages. There are synchronous and asynchronous message handlers.

### Synchronous message handlers

`IMessageHandler` consists of one method `Handle` that gets a message in a string format. You can deserialize that message (if it is a json message) or handle its raw value.
Thus, a message handler will look like this.

```c#
public class CustomMessageHandler : IMessageHandler
{
    public void Handle(string message, string routingKey)
    {
        // Do whatever you want.
        var messageObject = JsonConvert.DeserializeObject<YourClass>(message);
    }
}
```

You can also inject almost any services inside the `IMessageHandler` constructor.

```c#
public class CustomMessageHandler : IMessageHandler
{
    readonly ILogger<CustomMessageHandler> _logger;
    public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(string message, string routingKey)
    {
        _logger.LogInformation($"I got a message {message} by routing key {routingKey}");
    }
}
```

The only exception is the `IQueueService`. You can't inject it inside a message handler because of the appearance of cyclic dependencies. If you want to use an instance of `IQueueService` (e.g. handle one message and send another) use `INonCyclicMessageHandler`.
An example of `INonCyclicMessageHandler` will look like this.

```c#
public class CustomNonCyclicMessageHandler : INonCyclicMessageHandler
{
    readonly ILogger<CustomNonCyclicMessageHandler> _logger;
    public CustomNonCyclicMessageHandler(ILogger<CustomNonCyclicMessageHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(string message, string routingKey, IQueueService queueService)
    {
        _logger.LogInformation("Got a message. I will send it back to another queue.");
        var response = new { Message = message };
        queueService.Send(response, "exchange.name", "routing.key");
    }
}
```

### Asynchronous message handlers

`IMessageHandler` and `INonCyclicMessageHandler` work synchronously, but if you want to use an async technology then use `IAsyncMessageHandler` and `IAsyncNonCyclicMessageHandler`.

`IAsyncMessageHandler` will look like this.

```c#
public class CustomAsyncMessageHandler : IAsyncMessageHandler
{
    public async Task Handle(string message, string routingKey)
    {
        // Do whatever you want asynchronously!
    }
}
```

And `IAsyncNonCyclicMessageHandler` will be as in example below.

```c#
public class CustomAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
{
    readonly ILogger<CustomAsyncNonCyclicMessageHandler> _logger;

	// Injecting services is a privilege, you can leave it clean.
    public CustomAsyncNonCyclicMessageHandler(ILogger<CustomAsyncNonCyclicMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(string message, string routingKey, IQueueService queueService)
    {
        _logger.LogInformation("You can do something async, e.g. send message back.");
        var response = new { Message = message };
        await queueService.SendAsync(response, "exchange.name", "routing.key");
    }
}
```

So you can use async/await power inside your message handler.

### Message handlers registering

The third and final step is to register defined message handlers and let them "listen" for messages relying on specified rules. If there are no message handlers registered then received messages will not be processed.
You can register `IMessageHandler` in your `Startup` calling one of `AddMessageHandler`-ish methods. You are allowed to add message handlers in two modes, **singleton** or **transient**, and there are extension methods for each mode and each message handler type:

- `AddMessageHandlerTransient`
- `AddMessageHandlerSingleton`
- `AddNonCyclicMessageHandlerTransient`
- `AddNonCyclicMessageHandlerSingleton`
- `AddAsyncMessageHandlerTransient`
- `AddAsyncMessageHandlerSingleton`
- `AddAsyncNonCyclicMessageHandlerTransient`
- `AddAsyncNonCyclicMessageHandlerSingleton`

And this will look like this in your `Startup` code.

```c#
public class Startup
{
    public static IConfiguration Configuration;

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var clientConfiguration = Configuration.GetSection("RabbitMq");
        var exchangeConfiguration = Configuration.GetSection("RabbitMqExchange");
        services.AddRabbitMqClient(clientConfiguration)
            .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
            .AddMessageHandlerSingleton<CustomMessageHandler>("routing.key");
    }
}
```

RabbitMQ client and exchange configuration sections are not specified in this example, but covered [here](rabbit-configuration.md) and [here](exchange-configuration.md).

Message handlers can "listen" for messages by the **specified routing key**, or a **collection of routing keys**. If it is necessary, you can also register multiple message handler at once.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("first.routing.key")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>(new[] { "second.routing.key", "third.routing.key" });
```

You can also use **pattern matching** in routes where `*` (star) can substitute for exactly one word and `#` (hash) can substitute for zero or more words.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("*.routing.*")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>(new[] { "#.key", "third.*" });
```

You are also allowed to specify the exact exchange which will be "listened" by a message handler with the given routing key (or a pattern).

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("*.*.*", "ExchangeName")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("routing.key", "ExchangeName");
```

You can also set multiple message handlers for managing messages received by one routing key. This case can happen when you want to divide responsibilities between services (e.g. one contains business logic, and the other writes messages in the database).

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("first.routing.key")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("first.routing.key")
    .AddMessageHandlerSingleton<OneMoreCustomMessageHandler>("first.routing.key");
```

Since you are allowed to register multiple message handlers for one routing key (or one route pattern) you might want to make it run in a special order. You are allowed to do that too.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("first.routing.key", order: 1)
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("first.routing.key", order: 20)
    .AddMessageHandlerSingleton<OneMoreCustomMessageHandler>("first.routing.key", order: 300);
```

The higher order value - the more important message handler is. So it the previous code snippet the `OneMoreCustomMessageHandler` will process the received message first, `AnotherCustomMessageHandler` will be the second and `CustomMessageHandler` will be the third one.

You can also combine exchange and order configurations together!
```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("first.routing.key", "an.exchange", order: 1)
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("first.routing.key", "an.exchange", order: 20)
    .AddMessageHandlerSingleton<OneMoreCustomMessageHandler>("first.routing.key", "an.exchange", order: 300)
    .AddMessageHandlerSingleton<SecondMessageHandler>("second.routing.key", "other.exchange", order: 10)
    .AddMessageHandlerSingleton<ThirdMessageHandler>("second.routing.key", "other.exchange", order: 20);
```

### Workflow of message handling

The message handling process is organized as follows:

- `IQueueMessage` receives a message and delegates it to `IMessageHandlingService`.
- `IMessageHandlingService` gets a message (as a byte array) and decodes it to the UTF8 string. It also checks if there are any message handlers in a combined collection of `IMessageHandler`, `IAsyncMessageHandler`, `INonCyclicMessageHandler` and `IAsyncNonCyclicMessageHandler` instances and forwards a message to them.
- All subscribed message handlers (`IMessageHandler`, `IAsyncMessageHandler`, `INonCyclicMessageHandler`, `IAsyncNonCyclicMessageHandler`) process the given message in a given or a default order.
- `IMessageHandlingService` acknowledges the message by its `DeliveryTag`.
- If any exception occurs `IMessageHandlingService` acknowledges the message anyway and checks if the message has to be re-send. If exchange option `RequeueFailedMessages` is set `true` then `IMessageHandlingService` adds a header `"requeued"` to the message and sends it again with delay in 60 seconds. Mechanism of sending delayed messages covered in the message production [documentation](message-production.md).
- If any exception occurs within handling the message that has been already re-sent that message will not be re-send again (re-send happens only once).

### Batch message handlers

There are also a feature that you can use in case of necessity of handling messages in batches.
First of all you have to create a class that inherits a `BatchMessageHandler` class.
You have to set up values for `QueueName` and `PrefetchCount` properties. These values are responsible for the queue that will be read by the message handler, and the size of batches of messages.

```c#
public class CustomBatchMessageHandler : BatchMessageHandler
{
    readonly ILogger<CustomBatchMessageHandler> _logger;
    
    public CustomBatchMessageHandler(
        IRabbitMqConnectionFactory rabbitMqConnectionFactory,
        IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
        ILogger<CustomBatchMessageHandler> logger)
        : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, logger)
    {
        _logger = logger;
    }

    public override ushort PrefetchCount { get; set; } = 50;
    
    public override string QueueName { get; set; } = "another.queue.name";
    
    public override Task HandleMessages(IEnumerable<string> messages, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling a batch of messages.");
        foreach (var message in messages)
        {
            _logger.LogInformation(message);
        }
        return Task.CompletedTask;
    }
}
```

If you want to get raw messages as `ReadOnlyMemory<byte>` you can inherit base message handler class.

```c#
public class CustomBatchMessageHandler : BaseBatchMessageHandler
{
    readonly ILogger<CustomBatchMessageHandler> _logger;
    
    public CustomBatchMessageHandler(
        IRabbitMqConnectionFactory rabbitMqConnectionFactory,
        IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
        ILogger<CustomBatchMessageHandler> logger)
        : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, logger)
    {
        _logger = logger;
    }
    
    public override ushort PrefetchCount { get; set; } = 3;

    public override string QueueName { get; set; } = "queue.name";
    
    public override Task HandleMessages(IEnumerable<ReadOnlyMemory<byte>> messages, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling a batch of messages.");
        foreach (var message in messages)
        {
            var stringifiedMessage = Encoding.UTF8.GetString(message.ToArray());
            _logger.LogInformation(stringifiedMessage);
        }
        return Task.CompletedTask;
    }
}
```

After all you have to register that batch message handler via DI.
```c#
services.AddBatchMessageHandler<CustomBatchMessageHandler>(Configuration.GetSection("RabbitMq"));
```

The message handler will create a separate connection and use it for reading messages.
When the message collection is full to the size of `PrefetchCount` they are passed to the `HandleMessage` method.
Both `BaseBatchMessageHandler` and `BatchMessageHandler` implement `IDisposable` interface, so you can use it for release of resources.

Use this method of getting messages only when you sure that the number of messages that pass through this queue is really huge. Otherwise, messages could stack in the temporary collection of messages waiting to get in full.

For message production features see the [Previous page](message-production.md)

For more information about advanced usage of the RabbitMQ client see the [Next page](advanced-usage.md)