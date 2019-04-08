# RabbitMQ.Client.Core.DependencyInjection

This is a wrapper-library of RabbitMQ.Client with Dependency Injection infrastructure under the .Net Core 2.2 platform.

### Producer

First of all you need to add all service dependencies in the *ConfigureServices*. *AddRabbitMqClient* adds IQueueService that can send messages and *AddExchange* configures and adds an exchange. You can add multiple exchanges but the queue service will be single (and it will be added as singleton obviously).

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

If you using a console application then you can get an instance of the queue service like this:

```csharp
var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var queueService = serviceProvider.GetRequiredService<IQueueService>();
```

Or you can inject that queue service inside any class (service/controller/whatever) like this:

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

And now you can send messages using *Send* and *SendAsync* methods like this:
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

Or you can send a message with delay.
```csharp
queueService.Send(
       @object: messageObject,
       exchangeName: "exchange.name",
       routingKey: "routing.key",
	   secondsDelay: 10);
```

In order to make this possible, a default dead-letter-exchange exchanger with `"default.dlx.exchange"` name will be created. You can change it via main exchange configuration (example is down below).
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
The key point is adding custom message handlers by implementing *IMessageHandler* interface and adding it in *AddMessageHandlerSingleton<T>* method.
After configuring the queueService you need to start "listening" by simply calling *StartConsuming* method. After that you can get messages and handle it in any way you want.

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
		_logger.LogInformation("Ho-ho-hoooo");
		// Do whatever you want!
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

### appsettings.json configuration

 You have to have couple configuration sections - settings to connect to the RabbitMQ server and one section per exchange.
 Exchange sections define how to bind queues and exchanges and which routing keys to use.
 You can bind a queue to an exchange with more that one routing key, but if there are no routing keys in the queue section, then that queue will be bind to the exchange with its name.
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

"Type", "Durable", "AutoDelete", "DeadLetterExchange", "RequeueFailedMessages" are set with default values in this example. So you can change it or leave it like this:
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