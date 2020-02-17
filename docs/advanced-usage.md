# Advanced usage

`IQueueService` is an interface that implements two other interfaces - `IConsumingService` and `IProducingService`. By default a RabbitMQ Client is registered as `IQueueService` without a logical separation at producing and consuming code. Thus, you can inject only a `IQueueService` instance, and `IConsumingService` or `IProducingService` won't be available. This is not a real deal until you want to control the way a RabbitMQ Client connects to the server.

An instance of `IQueueService` opens two connections to the RabbitMQ server, one is for message production and the other one is for message consumption. Normally a RabbitMQ Client is added in a singleton mode, so both connections stay opened while application is running. It is also noticeable that a RabbitMQ Client uses the same credentials for both connections. If you add `IQueueService` in a transient mode (via the `AddRabbitMqClientTransient` extension method) both connections will be opened each time `IQueueService` is being injected somewhere else. This behavior does not fit everybody, so you can change it a little.

You are allowed to register a RabbitMQ Client as an implementation of two interfaces that have been mentioned before - `IConsumingService` and `IProducingService`. Each interface defines its own connection and its own collection of methods, obviously, for message production and message consumption. You can also use different credentials for different connections, and there is an option `ClientProvidedName` which allows you to create a "named" connection (which will be easier to find in the RabbitMQ management UI). There is also a possibility of registering `IConsumingService` and `IProducingService` in different lifetime modes, in case you want your consumption connection to be persist (singleton `IConsumingService`) and open a connection each time you want to send a message (a transient `IProducingService`). This situation will be covered in code examples below.

Let' say your application is a web API and you want to use both `IConsumingService` and `IProducingService`. Your `Startup` will look like this.

```c#
public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();

        // We will use different credentials for connections.
        var rabbitMqConsumerSection = Configuration.GetSection("RabbitMqConsumer");
        var rabbitMqProducerSection = Configuration.GetSection("RabbitMqProducer");

        // And we also configure different exchanges just for a better example.
        var producingExchangeSection = Configuration.GetSection("ProducingExchange");
        var consumingExchangeSection = Configuration.GetSection("ConsumingExchange");

        services.AddRabbitMqConsumingClientSingleton(rabbitMqConsumerSection)
            .AddRabbitMqProducingClientSingleton(rabbitMqProducerSection)
            .AddProductionExchange("exchange.to.send.messages.only", producingExchangeSection)
            .AddConsumptionExchange("consumption.exchange", consumingExchangeSection)
            .AddMessageHandlerTransient<CustomMessageHandler>("routing.key");

        services.AddHostedService<ConsumingHostedService>();
    }

    public void Configure(IApplicationBuilder app)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}
```

We have added `IConsumingService` and `IProducingService` via `AddRabbitMqConsumingClientSingleton` and `AddRabbitMqProducingClientSingleton` extension methods. We have also added two exchanges (for different purposes) via `AddProductionExchange` and `AddConsumptionExchange` methods which are covered in previous documentation sections. To start a message consumption we add a custom `IHostedService`, which injects `IConsumingService` and uses its `StartConsuming` method.

```c#
public class ConsumingHostedService : IHostedService
{
    readonly IConsumingService _consumingService;

    public ConsumingHostedService(IConsumingService consumingService)
    {
        _consumingService = consumingService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _consumingService.StartConsuming();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
```

To send messages we can only use `IProducingService`. Let's inject it inside a controller.

```c#
[ApiController]
[Route("api/example")]
public class ExampleController : ControllerBase
{
    readonly ILogger<ExampleController> _logger;
    readonly IProducingService _producingService;

    public ExampleController(
        IProducingService producingService,
        ILogger<ExampleController> logger)
    {
        _producingService = producingService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        _logger.LogInformation($"Sending messages with {typeof(IProducingService)}.");
        var message = new { message = "text" };
        await _producingService.SendAsync(message, "exchange.to.send.messages.only", "some.routing.key");
        return Ok(message);
    }
}
```

And the last thing we have to look at is a configuration file.
```
{
  "RabbitMqConsumer": {
    "ClientProvidedName": "Consumer",
    "TcpEndpoints": [
      {
        "HostName": "127.0.0.1",
        "Port": 5672
      }
    ],
    "Port": "5672",
    "UserName": "user-consumer",
    "Password": "passwordForConsumer"
  },
  "RabbitMqProducer": {
    "ClientProvidedName": "Producer",
    "TcpEndpoints": [
      {
        "HostName": "127.0.0.1",
        "Port": 5672
      }
    ],
    "Port": "5672",
    "UserName": "user-producer",
    "Password": "passwordForProducer"
  },
  "ConsumingExchange": {
    "Queues": [
      {
        "Name": "consuming.queue",
        "RoutingKeys": [ "routing.key" ]
      }
    ]
  },
  "ProducingExchange": {
    "Queues": [
      {
        "Name": "queue.of.producing.exchange",
        "RoutingKeys": [ "produce.messages", "produce.events" ]
      }
    ]
  }
}
```

As you can see, we set up a RabbitMQ client, which will create a connection for message production each time we call a `IProducingService`. We have also configured connections with different names and credentials. And the most important part is that we separated `IQueueService` for a logical parts. Be aware that `IQueueService` won't be available for injecting when you configure a RabbitMQ client in a `IConsumingService` plus `IProducingService` way.

For basic message consumption features see the [Previous page](message-consumption.md)