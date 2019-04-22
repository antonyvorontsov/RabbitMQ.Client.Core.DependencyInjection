# RabbitMQ.Client.Core.DependencyInjection

Wrapper-library of RabbitMQ.Client with Dependency Injection infrastructure under the .Net Core 2.2 platform. <a href="https://www.nuget.org/packages/RabbitMQ.Client.Core.DependencyInjection/" alt="NuGet package"><img src="https://img.shields.io/nuget/v/RabbitMQ.Client.Core.DependencyInjection.svg" /></a>

### Producer

First of all you have to add all service dependencies in the `ConfigureServices` method. `AddRabbitMqClient` adds `IQueueService` that can send messages and `AddExchange` configures and adds an exchange. You can add multiple exchanges but the queue service will be single (and it will be added as singleton obviously).

```csharp

public static IConfiguration Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var rabbitMqSection = Configuration.GetSection("RabbitMq");
    var exchangeSection = Configuration.GetSection("RabbitMqExchange");

    services.AddRabbitMqClient(rabbitMqSection)
        .AddExchange("exchange.name", exchangeSection);
}
```

If you are using a console application then you can get an instance of the queue service like this:

```csharp
var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var queueService = serviceProvider.GetRequiredService<IQueueService>();
```

Or you can inject that queue service inside anything (service/controller/whatever):

```csharp
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

Now you can send messages using `Send` and `SendAsync` methods:

```csharp
var messageObject = new
{
	Id = 1,
	Name = "RandomName"
};

queueService.Send(
       @object: messageObject,
       exchangeName: "exchange.name",
       routingKey: "routing.key");
```

There is a bunch of different methods like `SendJson` or `SendString` with their async versions but the `Send` method allows you to send any object of any type and makes communication process kinda easier.
Any message will be persistent and sent with `application/json` Content Type. Only exception is `SendString` method that was written just in case you want to send something of your own (e.g. xml).

You can also send messages with delay.
```csharp
queueService.Send(
	@object: messageObject,
	exchangeName: "exchange.name",
	routingKey: "routing.key",
	secondsDelay: 10);
```

In order to make this possible, a default dead-letter-exchange with `"default.dlx.exchange"` name will be created. You can change it via main exchange configuration (example is down below).
And also you have a default functionality of resending failed messages (if you get an error while processing recieved message).

### Consumer

Lets imagine that you wanna make a consumer as a console application. Then code will look like this:

```csharp
class Program
{
	const string ExchangeName = "exchange.name";
	public static IConfiguration Configuration { get; set; }

	static void Main(string[] args)
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
			.AddExchange("exchange.name", exchangeSection)
			.AddMessageHandlerSingleton<CustomMessageHandler>("routing.key")
			.AddAsyncMessageHandlerSingleton<CustomAsyncMessageHandler>("other.routing.key");
	}
}
```

You have to configure QueueService the same way as you've done with producer.
The key point is adding custom message handlers by implementing `IMessageHandler` interface and adding it in `AddMessageHandlerSingleton<T>` or `AddMessageHandlerTransient<T>` methods.
After configuring the queueService you have to start "listening" by simply calling `StartConsuming` method of `IQueueService`. After that you can get messages and handle them in any way you want.

Message handler example:
```csharp
public class CustomMessageHandler : IMessageHandler
{
	readonly ILogger<CustomMessageHandler> _logger;
	public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
	{
		_logger = logger;
	}

	public void Handle(string message, string routingKey)
	{
		// Do whatever you want!
		_logger.LogInformation("Ho-ho-hoooo");
	}
}
```

Or you can add another message handler that will run asynchronously:
```csharp
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
		_logger.LogInformation("Merry christmas!");
	}
}
```

But you can not use `IQueueService` inside those message handlers otherwise you will be faced with cycling dependency problem. But sometimes you may need to send something in other queue (e.g. queue with responses) from one message handler or another. For that purpose use non-cyclinc handlers.

```csharp
public class CustomMessageHandler : INonCyclicMessageHandler
{
	readonly ILogger<CustomMessageHandler> _logger;
	public CustomMessageHandler(ILogger<CustomMessageHandler> logger)
	{
		_logger = logger;
	}

	public void Handle(string message, string routingKey, IQueueService queueService)
	{
		_logger.LogInformation("Got a message.");
		var response = new { Message = message };
		queueService.Send(response, "exchange.name", "routing.key");
	}
}
```

Or the same but async.

```csharp
public class CustomAsyncMessageHandler : IAsyncNonCyclicMessageHandler
{
	readonly ILogger<CustomAsyncMessageHandler> _logger;

	public CustomAsyncMessageHandler(ILogger<CustomAsyncMessageHandler> logger)
	{
		_logger = logger;
	}

	public async Task Handle(string message, string routingKey, IQueueService queueService)
	{
		_logger.LogInformation("Doing something async.");
		var response = new { Message = message };
		await queueService.SendAsync(response, "exchange.name", "routing.key");
	}
}
```

And you have to register those classes the same way you did with simple handlers.
```csharp
services.AddRabbitMqClient(rabbitMqSection)
	.AddExchange("exchange.name", exchangeSection)
	.AddNonCyclicMessageHandlerSingleton<CustomMessageHandler>("routing.key")
	.AddAsyncNonCyclicMessageHandlerSingleton<CustomAsyncMessageHandler>("other.routing.key");
```

You can find example projects in the repository too.

### appsettings.json configuration

 You have to add a couple configuration sections: (1) settings to connect to the RabbitMQ server and (2) a section that configures an exchange (one section per exchange frankly speaking).
 Exchange sections define how to bind queues and exchanges with each ohter and which routing keys to use for that.
 You can bind a queue to an exchange with more than one routing key, but if there are no routing keys in the queue section, then that queue will be bound to the exchange with its name.
```
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
	"RequeueFailedMessages": true
    "Queues": [
	  {
        "Name": "myqueue",
        "RoutingKeys": [ "routing.key" ]
      }
    ]
  }
}
```

`Type`, `Durable`, `AutoDelete`, `DeadLetterExchange`, `RequeueFailedMessages` are set with default values in this example. So you can change it or leave it like this:
```
{
  "RabbitMq": {
    "HostName": "127.0.0.1",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  },
  "RabbitMqExchange": {
    "Queues": [
	  {
        "Name": "myqueue",
        "RoutingKeys": [ "routing.key" ]
      }
    ]
  }
}
```