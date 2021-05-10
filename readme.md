# RabbitMQ.Client.Core.DependencyInjection

<a href="https://www.nuget.org/packages/RabbitMQ.Client.Core.DependencyInjection/" alt="NuGet package"><img src="https://img.shields.io/nuget/v/RabbitMQ.Client.Core.DependencyInjection.svg" /></a><br/>
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f688764d2ba340099ec50b74726e25fd)](https://app.codacy.com/app/AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection?utm_source=github.com&utm_medium=referral&utm_content=AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection&utm_campaign=Badge_Grade_Dashboard)<br/>

This repository contains the library that provides functionality for wrapping [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client) code and adding it in your application via the dependency injection mechanism. That wrapper provides easy, managed message queue consume and publish operations. The library targets netstandard2.1.

## Usage

This section contains only an example of a basic usage of the library. You can find the [detailed documentation](./docs/index.md) in the docs directory where all functionality fully covered.

### Producer

To produce messages to RabbitMQ queues you have to go through the routine of configuring RabbitMQ connection properties and exchanges as well. In your `Startup` file you can do it simply calling a couple of methods in a fluent-api way.

```c#
public static IConfiguration Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var rabbitMqSection = Configuration.GetSection("RabbitMq");
    var exchangeSection = Configuration.GetSection("RabbitMqExchange");

    services.AddRabbitMqServices(rabbitMqSection)
        .AddExchange("exchange.name", exchangeSection);
}
```

`AddRabbitMqServices` adds `IProducingService` and `IConsumingService` (will be covered in the next section) that provide functionality of producing and consuming messages respectively. `AddExchange` configures one exchange with queue bindings that allow messages to route properly.
An example of `appsettings.json` is two sections below. You can also configure everything manually passing an instance of the `RabbitMqExchangeOptions` class. For more information, see [rabbit-configuration](./docs/rabbit-configuration.md) and [exchange-configuration](./docs/exchange-configuration.md) documentation files.

Now you can inject an instance of `IProducingService` inside anything you want (e.g. simple API controller).

```c#
[Route("api/[controller]")]
public class HomeController : Controller
{
    private readonly IProducingService _producingService;
    public HomeController(IProducingService producingService)
    {
        _producingService = producingService;
    }
}
```

`IProducingService` provides different overloads of `Send` and `SendAsync` methods.

```c#
var messageObject = new
{
    Id = 1,
    Name = "RandomName"
};

await _producingService.SendAsync(
    @object: messageObject,
    exchangeName: "exchange.name",
    routingKey: "routing.key");
```

You can also send messages with delay.

```c#
await _producingService.SendAsync(
    @object: messageObject,
    exchangeName: "exchange.name",
    routingKey: "routing.key",
    millisecondsDelay: 1500);
```

 The mechanism of sending delayed messages is described in the [documentation](./docs/message-production.md). Dive into it for more detailed information.

### Consumer

The second part of working with AMPQ is message consumption. Let's assume that you have an API that also consumes messages from RabbitMQ.

```c#
public static IConfiguration Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var rabbitMqSection = Configuration.GetSection("RabbitMq");
    var exchangeSection = Configuration.GetSection("RabbitMqExchange");

    services.AddRabbitMqServices(rabbitMqSection)
        .AddExchange("exchange.name", exchangeSection)
        .AddMessageHandlerSingleton<CustomMessageHandler>("routing.key");
}
```

You have to configure everything almost the same way as you have already done previously with the producer.
`AddExchange` method configures an exchange both for production and consumption. but in case you do not want to produce messages to that queue you can alternative method `AddConsumptionExchange` instead. For more detailed information about difference in exchange declarations you may want to see the [documentation](./docs/exchange-configuration.md).
The other important part is adding custom message handlers by implementing the `IMessageHandler` interface and calling `AddMessageHandlerSingleton<T>` or `AddMessageHandlerTransient<T>` methods. `IMessageHandler` is a simple subscriber, which receives messages from a queue by selected routing key. You are allowed to set multiple message handlers for one routing key (e.g. one is writing it in a database, and the other does the business logic).

You can also use **pattern matching** while adding message handlers where `*` (star) can substitute for exactly one word and `#` (hash) can substitute for zero or more words.
You are also allowed to specify the exact exchange which will be "listened" by the selected message handler with the given routing key (or a pattern).

Message consumption will be started automatically by the hosted service that is registered inside the `AddRabbitMqServices` method. That hosted services uses `IConsumingService` implementation that is responsible for message consumption basically. So you do not have to start consumption manually.

```c#
services.AddRabbitMqServices(rabbitMqSection)
    .AddConsumptionExchange("exchange.name", exchangeSection)
    .AddMessageHandlerSingleton<CustomMessageHandler>("*.*.key")
    .AddMessageHandlerTransient<AnotherCustomMessageHandler>("#", "exchange.name");
```

A message handler example.

```c#
public class CustomMessageHandler : IMessageHandler
{
    readonly ILogger<CustomMessageHandler> _logger;
    public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
    {
        _logger = logger;
    }

    public void Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
    {
        // Do whatever you want with the message.
        _logger.LogInformation("Hello world");
    }
}
```

If you want to use an async version of the handler then implement your custom `IAsyncMessageHandler`.

```c#
public class CustomAsyncMessageHandler : IAsyncMessageHandler
{
    readonly ILogger<CustomAsyncMessageHandler> _logger;
    public CustomAsyncMessageHandler(ILogger<CustomAsyncMessageHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(BasicDeliverEventArgs eventArgs, string matchingRoute)
    {
	   // await something.
    }
}
```

You can register any of those message handlers the same way you registered `IMessageHandler` before. There are similar extension methods for each type of message handlers. For more information, see the [message-consuming](./docs/message-consumption.md) documentation file.

You can also find example projects in the repository inside the [examples](./examples) directory.

### Configuration

In both cases (described earlier) for producing and consuming messages configurations are the same. `appsettings.json` consists of those sections: (1) settings to connect to the RabbitMQ server and (2) sections that configure exchanges and queue bindings. You can have multiple exchanges and one configuration section per exchange.
Exchange sections define how to bind queues and exchanges with each other using specified routing keys (or matching patterns). You allowed to bind a queue to an exchange with more than one routing key, but if there are no routing keys in the queue section, then that queue will be bound to the exchange with its name.

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
First of all you have to create a class that inherits a `BaseBatchMessageHandler` class.
You have to set up values for `QueueName` and `PrefetchCount` properties. These values are responsible for the queue that will be read by the message handler, and the size of batches of messages.

```c#
public class CustomBatchMessageHandler : BaseBatchMessageHandler
{
    private readonly ILogger<CustomBatchMessageHandler> _logger;

    public CustomBatchMessageHandler(
        IRabbitMqConnectionFactory rabbitMqConnectionFactory,
        IEnumerable<BatchConsumerConnectionOptions> batchConsumerConnectionOptions,
        IEnumerable<IBatchMessageHandlingMiddleware> batchMessageHandlingMiddlewares,
        ILoggingService loggingService,
        ILogger<CustomBatchMessageHandler> logger)
        : base(rabbitMqConnectionFactory, batchConsumerConnectionOptions, batchMessageHandlingMiddlewares, loggingService)
    {
        _logger = logger;
    }

    public override ushort PrefetchCount { get; set; } = 3;

    // You have to be aware that BaseBatchMessageHandler does not declare the specified queue. So if it does not exists an exception will be thrown.
    public override string QueueName { get; set; } = "queue.name";

    public override Task HandleMessages(IEnumerable<BasicDeliverEventArgs> messages, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling a batch of messages.");
        foreach (var message in messages)
        {
            _logger.LogInformation(message.GetMessage());
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
When the message collection is full to the size of `PrefetchCount` they are passed to the `HandleMessage` method. You can also set a timespan `MessageHandlingPeriod` property value and the method `HandleMessage` will be executed repeatedly over time so messages in unfilled batches could be processed too. For more information, see the [message-consuming](./docs/message-consumption.md) documentation file.

## Advanced usage and nuances

RabbitMQ services implemented in this library open two connections to the RabbitMQ server. One connection is used for message production, and the other one is for message consumption.
This behavior covered in the [advanced usage documentation file](./docs/advanced-usage.md), dive into it deeply if you want to control the client behavior tighter.

There is also an [example project](./examples/Examples.AdvancedConfiguration) that demonstrates an advanced usage of the RabbitMQ client.

## Changelog

All notable changes covered in the [changelog](./docs/changelog.md) file.

## License

This library licensed under the MIT license.

Feel free to contribute!