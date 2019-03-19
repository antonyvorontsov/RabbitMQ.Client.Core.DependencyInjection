# RabbitMQ.Client.Core

This is a wrapper-library of RabbitMQ.Client with Dependency Injection infrastructure under the .Net Core 2.2 platform.

### Producer

!TBD

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

!TBD

```csharp
var serviceCollection = new ServiceCollection();
ConfigureServices(serviceCollection);
var serviceProvider = serviceCollection.BuildServiceProvider();
var queueService = serviceProvider.GetRequiredService<IQueueService>();

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

### Consumer

!TBD

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
            .AddClientLogger<CustomLogger>();
       }
}
```

Message handler example:

```csharp
public class CustomMessageHandler : IMessageHandler
{
    public IEnumerable<string> RoutingKeys { get; set; }
    public void Handle(string message, string routingKey)
    {
		// Do something.
    }
}
```

 You can also add custom loggers to handle log messages which comes from the library with your own business logic.
```csharp
public class CustomLogger : ILogger
{
    public void Log(Log​Level logLevel, string message)
    {
		// Some custom logic. You can write logs to the database for example.
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
       "Queues": [
         {
             "Name": "myqueue",
             "RoutingKeys": [
               "routing.key"
             ]
         }
       ]
  }
}
```