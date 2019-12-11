# RabbitMQ.Client.Core.DependencyInjection

<a href="https://www.nuget.org/packages/RabbitMQ.Client.Core.DependencyInjection/" alt="NuGet package"><img src="https://img.shields.io/nuget/v/RabbitMQ.Client.Core.DependencyInjection.svg" /></a><br/>
[![Codacy Badge](https://api.codacy.com/project/badge/Grade/f688764d2ba340099ec50b74726e25fd)](https://app.codacy.com/app/AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection?utm_source=github.com&utm_medium=referral&utm_content=AntonyVorontsov/RabbitMQ.Client.Core.DependencyInjection&utm_campaign=Badge_Grade_Dashboard)<br/>
This repository contains the library that provides functionality to wrap [RabbitMQ.Client](https://github.com/rabbitmq/rabbitmq-dotnet-client) code and register it via dependency injection mechanism.

## Usage
TODO: link to detailed documentation.
### Producer

There are some step that you have to get through inside the `ConfigureServices` method for basic RabbitMQ configuration. The first mandatory step is to add `IQueueService` that contains all the logic of producing and consuming messages by calling `AddRabbitMqClient` method.
The second step is add and configure exchanges using `AddExchange`, `AddProductionExchange` and `AddConsumptionExchange` methods. Exchanges have an option (flag) are the made to consume messages or only produce them. This is an important case when you want to use multiple exchanges in your application and want to consume messages from queues binded to chosen exchanges.
So if you want to add an exchange and only produce messages, then use `AddProductionExchange` method. If you want to use full functionality, then use `AddConsumptionExchange` method. Or you can do any of them using `AddExchange` method and passing `isConsuming` parameter. Examples are provided.
You can add multiple exchanges but the queue service will be added as singleton.

After those two steps you are good to go. Down below is an example of the basic RabbitMQ configuration.
```csharp

public static IConfiguration Configuration { get; set; }

public void ConfigureServices(IServiceCollection services)
{
    var rabbitMqSection = Configuration.GetSection("RabbitMq");
    var exchangeSection = Configuration.GetSection("RabbitMqExchange");

    services.AddRabbitMqClient(rabbitMqSection)
        .AddProductionExchange("exchange.name", exchangeSection);
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
And also you have a default functionality of resending failed messages (if you get an error while processing received message).

### Consumer

Lets imagine that you wanna make a consumer as a console application. Then code will look like this:

```csharp
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

You can find example projects in the repository too.

### Configuration

 You have to add  configuration sections: (1) settings to connect to the RabbitMQ server and (2) sections that configure exchanges.
 Exchange sections define how to bind queues and exchanges with each other and which routing keys to use for that.
 You can bind a queue to an exchange with more than one routing key, but if there are no routing keys in the queue section, then that queue will be bound to the exchange with its name.
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

## Versioning
Say something about semantic versioning.

## Changelog

All notable changes being tracked in the [changelog](./docs/changelog.md) file.

## License
This library is licenced under GNU General Public License v3 that means you are free to use it anywhere you want but you have to provide to the community all modifying changes of the library.
Also feel free to contribute!