# RabbitMQ.Client.Core.DependencyInjection

<a href="https://www.nuget.org/packages/RabbitMQ.Client.Core.DependencyInjection/" alt="NuGet package"><img src="https://img.shields.io/nuget/v/RabbitMQ.Client.Core.DependencyInjection.svg" /></a><br/>
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f688764d2ba340099ec50b74726e25fd)](https://app.codacy.com/app/AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection?utm_source=github.com&utm_medium=referral&utm_content=AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection&utm_campaign=Badge_Grade_Dashboard)<br/>

This repository contains the library that provides functionality for wrapping [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client) code and adding it in your application via the dependency injection mechanism. That wrapper provides easy, managed message queue consume and publish operations. The library targets netstandard2.1.

## Usage

This section contains only an example of a basic usage of the library. You can find the [detailed documentation](./docs/index.md) in the docs directory where all functionality fully covered.

### Producer

To produce messages in the RabbitMQ queue you have to go through the routine of configuring a RabbitMQ connection and exchanges. In your `Startup` file you can do it simply calling couple methods in a fluent-Api way.

```c#
public static IConfiguration Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var rabbitMqSection = Configuration.GetSection("RabbitMq");
    var exchangeSection = Configuration.GetSection("RabbitMqExchange");

    services.AddRabbitMqClient(rabbitMqSection)
        .AddProductionExchange("exchange.name", exchangeSection);
}
```

By calling `AddRabbitMqClient` you add `IQueueService` that provides functionality of sending messages to queues. `AddProductionExchange` configures exchange to queues bindings (presented in json configuration) that allow messages to route properly.
Example of `appsettings.json` is two sections below. You can also configure everything manually. For more information, see [rabbit-configuration](./docs/rabbit-configuration.md) and [exchange-configuration](./docs/exchange-configuration.md) documentation files.

Now you can inject an instance of `IQueueService` inside anything you want.

```c#
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly IQueueService _queueService;
    public HomeController(IQueueService queueService)
    {
        _queueService = queueService;
    }
}
```

Now you can send messages using `Send` or `SendAsync` methods.

```c#
var messageObject = new
{
    Id = 1,
    Name = "RandomName"
};

_queueService.Send(
    @object: messageObject,
    exchangeName: "exchange.name",
    routingKey: "routing.key");
```

You can also send delayed messages.

```c#
_queueService.Send(
    @object: messageObject,
    exchangeName: "exchange.name",
    routingKey: "routing.key",
    millisecondsDelay: 10);
```

 The mechanism of sending delayed messages described in the [documentation](./docs/message-production.md). Dive into it for more detailed information.

### Consumer

After making the message production possible let's make the consumption possible too! Imagine that a consumer is a simple console application.

```c#
class Program
{
    const string ExchangeName = "exchange.name";
    public static IConfiguration Configuration { get; set; }

    static void Main()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        Configuration = builder.Build();

        var serviceCollection = new ServiceCollection();
        ConfigureServices(serviceCollection);

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var queueService = serviceProvider.GetRequiredService<IQueueService>();
        queueService.StartConsuming();
    }

    static void ConfigureServices(IServiceCollection services)
    {
        var rabbitMqSection = Configuration.GetSection("RabbitMq");
        var exchangeSection = Configuration.GetSection("RabbitMqExchange");

        services.AddRabbitMqClient(rabbitMqSection)
            .AddConsumptionExchange("exchange.name", exchangeSection)
            .AddMessageHandlerSingleton<CustomMessageHandler>("routing.key");
    }
}
```

You have to configure everything almost the same way as you have already done previously with the producer. The main differences are that you need to declare (configure) a consumption exchange calling `AddConsumptionExchange` instead of a production exchange. For detailed information about difference in exchange declarations you may want to see the [documentation](./docs/exchange-configuration.md).
The other important part is adding custom message handlers by implementing the `IMessageHandler` interface and calling `AddMessageHandlerSingleton<T>` or `AddMessageHandlerTransient<T>` methods. `IMessageHandler` is a simple subscriber, which receives messages from a queue by selected routing key. You are allowed to set multiple message handlers for one routing key (e.g. one is writing it in a database, and the other does the business logic).

You can also use **pattern matching** while adding message handlers where `*` (star) can substitute for exactly one word and `#` (hash) can substitute for zero or more words.
You are also allowed to specify the exact exchange which will be "listened" by the selected message handler with the given routing key (or a pattern).

```c#
services.AddRabbitMqClient(rabbitMqSection)
    .AddConsumptionExchange("exchange.name", exchangeSection)
    .AddMessageHandlerSingleton<CustomMessageHandler>("*.*.key")
    .AddMessageHandlerSingleton<AnotherCustomMessageHandler>("#", "exchange.name");
```

The very last step is to start "listening" (consuming) by simply calling `StartConsuming` method of `IQueueService`. After that you will start getting messages, and you can handle them in any way you want.

A message handler example.

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
        // Do whatever you want with the message.
        _logger.LogInformation("Hello world");
    }
}
```

If you want to use an async magic then implement your custom `IAsyncMessageHandler`.

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
	   // await something.
    }
}
```

If you want to send messages from the `Handle` method inside your message handler then use `INonCyclicMessageHandler`.
You can't inject  `IQueueService` inside any message handlers, otherwise you will get a cyclic dependency exception. But `INonCyclicMessageHandler` allows you to avoid this as it accepts an instance of `IQueueService` as a parameter of the method `Handle`.

```c#
public class MyNonCyclicMessageHandler : INonCyclicMessageHandler
{
    // Inject anything you want except IQueueService.
    readonly ILogger<MyNonCyclicMessageHandler> _logger;
    public MyNonCyclicMessageHandler(ILogger<MyNonCyclicMessageHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(string message, string routingKey, IQueueService queueService)
    {
        // Send anything you want using IQueueService instance.
        var anotherMessage = new MyMessage { Foo = "Bar" };
        queueService.Send(anotherMessage, "exchange.name", "routing.key");
    }
}
```

`INonCyclicMessageHandler` has its asynchronous analogue `IAsyncNonCyclicMessageHandler`.

```c#
public class MyAsyncNonCyclicMessageHandler : IAsyncNonCyclicMessageHandler
{
    // Inject anything you want except IQueueService.
    readonly ILogger<MyAsyncNonCyclicMessageHandler> _logger;
    public MyAsyncNonCyclicMessageHandler(ILogger<MyAsyncNonCyclicMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(string message, string routingKey, IQueueService queueService)
    {
        // Do async stuff.
        var anotherMessage = new MyMessage { Foo = "Bar" };
        await queueService.SendAsync(anotherMessage, "exchange.name", "routing.key");
    }
}
```

You can register any of those message handlers the same way you registered `IMessageHandler` before. There are similar extension methods for each type of message handlers. For more information, see the [message-consuming](./docs/message-consumption.md) documentation file.

You can also find example projects in the repository inside the [examples](./examples) directory.

### Configuration

In both cases for producing and consuming messages the configuration file is the same. `appsettings.json` consists of those sections: (1) settings to connect to the RabbitMQ server and (2) sections that configure exchanges and queue bindings. You can have multiple exchanges and one configuration section per each exchange.
Exchange sections define how to bind queues and exchanges with each other using specified routing keys (or patterns). You allowed to bind a queue to an exchange with more than one routing key, but if there are no routing keys in the queue section, then that queue will be bound to the exchange with its name.

```json
{
  "RabbitMq": {
    "HostName": "127.0.0.1",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  },
  "RabbitMqExchange": {
    "Type": "direct",
    "Durable": true,
    "AutoDelete": false,
    "DeadLetterExchange": "default.dlx.exchange",
    "RequeueFailedMessages": true,
    "Queues": [
	  {
        "Name": "myqueue",
        "RoutingKeys": [ "routing.key" ]
      }
    ]
  }
}
```

For more information about `appsettings.json` and manual configuration features, see [rabbit-configuration](./docs/rabbit-configuration.md) and [exchange-configuration](./docs/exchange-configuration.md) documentation files.

## Batch message handlers

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

    public override string QueueName { get; set; } = "queue.name";

    public override TimeSpan? MessageHandlingPeriod { get; set; } = TimeSpan.FromMilliseconds(500);

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

After all you have to register that batch message handler via DI.
```c#
services.AddBatchMessageHandler<CustomBatchMessageHandler>(Configuration.GetSection("RabbitMq"));
```

The message handler will create a separate connection and use it for reading messages.
When the message collection is full to the size of `PrefetchCount` they are passed to the `HandleMessage` method. You can also set a `MessageHandlingPeriod` property value and the method `HandleMessage` will be executed repeatedly so messages in unfilled batches could be processed too. For more information, see the [message-consuming](./docs/message-consumption.md) documentation file.

## Advanced usage and nuances

RabbitMQ client implemented in this library (class which implements `IQueueService`) opens two connections to the RabbitMQ server. One connection is used for message production, and the other one is for message consumption.
This behavior covered in the [advanced usage documentation file](./docs/advanced-usage.md), dive into it deeply if you want to control the client behavior tighter.

There is also an [example project](./examples/Examples.AdvancedConfiguration) that demonstrates an advanced usage of the RabbitMQ client.

## Changelog

All notable changes covered in the [changelog](./docs/changelog.md) file.

## License

This library licensed under the MIT license.

Feel free to contribute!