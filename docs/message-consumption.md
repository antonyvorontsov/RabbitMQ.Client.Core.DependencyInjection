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

### Synchronous message handlers

The second step without which receiving messages does not make sense - configuration of message handling services. If there are no message handlers then received messages will not be processed.

Message handlers are classes that implement the `IMessageHandler` interface (or a few others) and contain functionality (including error handling) for processing messages.
You can register `IMessageHandler` in your `Startup` like this.

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

`IMessageHandler` implementation will "listen" for messages by the specified routing key, or a collection of routing keys. If it is necessary, you can also register multiple message handler at once.

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

You can register it in two modes, **singleton** or **transient**, using `AddMessageHandlerSingleton` or `AddMessageHandlerTransient` methods respectively.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerTransient<CustomMessageHandler>(new[] { "#.key", "third.*" });
```

You can also set multiple message handlers for managing messages received by one routing key. This case can happen when you want to divide responsibilities between services (e.g. one contains business logic, and the other writes messages in the database).

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddMessageHandlerSingleton<CustomMessageHandler>("first.routing.key")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("first.routing.key")
    .AddMessageHandlerSingleton<OneMoreCustomMessageHandler>("first.routing.key");
```

`IMessageHandler` consists of one method `Handle` that gets a message in a string format. You can deserialize it (if it is a json message) or handle a raw value.
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

You can also inject services inside the `IMessageHandler` constructor.

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
`INonCyclicMessageHandler` can be registered the same way as `IMessageHandler`. There are similar semantic methods for adding it in **singleton** or **transient** modes.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddNonCyclicMessageHandlerTransient<CustomNonCyclicMessageHandler>("first.routing.key")
    .AddNonCyclicMessageHandlerSingleton<AnotherNonCyclicCustomMessageHandler>(new [] { "second.routing.key", "third.routing.key" });
```

And the code of `INonCyclicMessageHandler` will look like this.

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
There are extension methods that allow you to register it the same way as synchronous ones in **singleton** or **transient** modes.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration)
    .AddAsyncMessageHandlerTransient<CustomAsyncMessageHandler>("first.routing.key")
    .AddAsyncNonCyclicMessageHandlerSingleton<CustomAsyncNonCyclicMessageHandler>(new [] { "second.routing.key", "third.routing.key" });
```

`IAsyncMessageHandler` will look like this.

```c#
public class CustomAsyncMessageHandler : IAsyncMessageHandler
{
    readonly ILogger<CustomAsyncMessageHandler> _logger;

    public CustomAsyncMessageHandler(ILogger<CustomAsyncMessageHandler> logger)
    {
        _logger = logger;
    }

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

### Workflow of message handling

The message handling process is organized as follows:

- `IQueueMessage` receives a message and delegates it to `IMessageHandlingService`.
- `IMessageHandlingService` gets a message (as a byte array) and decodes it to the UTF8 string. It also checks if there are any message handlers in collections of `IMessageHandler`, `IAsyncMessageHandler`, `INonCyclicMessageHandler` and `IAsyncNonCyclicMessageHandler` instances and forwards a message to them.
- All subscribed message handlers (`IMessageHandler`, `IAsyncMessageHandler`, `INonCyclicMessageHandler`, `IAsyncNonCyclicMessageHandler`) process the given message.
- `IMessageHandlingService` acknowledges the message by its `DeliveryTag`.
- If any exception occurs `IMessageHandlingService` acknowledges the message anyway and checks if the message has to be re-send. If exchange option `RequeueFailedMessages` is set `true` then `IMessageHandlingService` adds a header `"requeued"` to the message and sends it again with delay in 60 seconds. Mechanism of sending delayed messages covered in the message production [documentation](message-production.md).
- If any exception occurs within handling the message that has been already re-sent that message will not be re-send again (re-send happens only once).

For message production features see the [Previous page](message-production.md)