# Exchange configuration

### Basics

Client applications work with exchanges and queues, and they must be "declared" and "bound" to each other is a certain way before they can be used.
Queues and exchanges can also be customized by using additional parameters. This library allows to do this routine simply calling `AddExchange` method passing additional parameters to it.
You are allowed to configure multiple exchanges with multiple queues bound to them.

Your `Startup` code will look like this.

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
    
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
    }
}
```

And the `appsettings.json` file will be like this.

```json
{
 "RabbitMqExchange": {
    "Type": "direct",
    "Durable": true,
    "AutoDelete": false,
    "DeadLetterExchange": "default.dlx.exchange",
    "RequeueFailedMessages": true,
    "Arguments": { "key": "value" },
    "Queues": [
      {
        "Name": "MyQueueName",
        "Durable": true,
        "AutoDelete": false,
        "Exclusive": false,
        "Arguments": { "key": "value" },
        "RoutingKeys": [ "first.routing.key", "second.routing.key" ]
      }
    ]
  }
}
```

The RabbitMQ client configuration section not specified in this example, for more information see the [documentation](rabbit-configuration.md) file.

Exchanges can be configured with properties:
- `Type`  - exchange type (direct, topic, fanout). Default value is `"direct"`.
- `Durable` - durability option. Default value is `true`.
- `AutoDelete` - option for exchange auto deleting. Default value is `false`.
- `Arguments` - dictionary of additional arguments. Default value is `null`.
- `RequeueFailedMessages` - option that specifies behaviour of re-queueing failed messages with delay through dead-letter-exchange. Default value is `true`. Mechanism of sending delayed messages covered in the [documentation](message-production.md).
- `DeadLetterExchange` - default value for dead-letter-exchange. Default value for dead-letter-exchange name is `"default.dlx.exchange"`.
- `Queues` - collection of queues bound to the exchange.

Queue options:
- `Name`  - queue name.
- `Durable` - durability option. Default value is `true`.
- `AutoDelete` - option for queue auto deleting. Default value is `false`.
- `Exclusive` - exclusive option. Default value is `false`.
- `Arguments` - dictionary of additional [arguments](https://www.rabbitmq.com/queues.html#optional-arguments). Default value is `null`.
- `RoutingKeys` - collection of routing keys that queue "listens".

Taking into account all the default values that you can skip configuration will look like this.

```json
{
 "RabbitMqExchange": {
    "Type": "direct",
    "Queues": [
      {
        "Name": "MyQueueName",
        "RoutingKeys": [ "first.routing.key", "second.routing.key" ]
      }
    ]
  }
}
```

If you want to use routing keys matching with queue names you can skip `"RoutingKeys"` option and queues will be bound to the exchange by their names.

```json
{
 "RabbitMqExchange": {
    "Type": "direct",
    "Queues": [
      {
        "Name": "queue.name.as.routing.key"
      }
    ]
  }
}
```

### Production and consumption exchanges

There are two custom "types" of exchanges in this library - **production** and **consumption**. **Production** exchanges supposed to be used only in apps that produce messages. **Consumption** exchanges are made for both production and consuming.
Why is it even necessary to have such division? You can control behaviour of set of exchanges in multi-purpose applications that consume as well as consume messages and avoid getting unwanted messages.

For defining custom "type" of exchange you have to set `isConsuming` parameter accepted by the `AddExchange` method. If the value is `true` then application will be getting (consuming) messages from queues bound to that exchange. If it is `false` exchange will only be used for producing messages.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddExchange("ExchangeName", isConsuming: true, exchangeConfiguration);
```

You can also use `AddConsumptionExchange` or `AddProductionExchange` but under the hood it is the same as using `AddExchange` method with the `isConsuming` parameter.

```c#
services.AddRabbitMqClient(clientConfiguration)
    .AddConsumptionExchange("ConsumptionExchange", exchangeConfiguration);

// And the other method.

services.AddRabbitMqClient(clientConfiguration)
    .AddProductionExchange("ProductionExchange", exchangeConfiguration);
```

### Manual configuring

You can also configure exchanges manually passing instances of `RabbitMqExchangeOptions` and `RabbitMqQueueOptions` classes to one of the `AddExchange` methods.
 
```c#
var exchangeOptions = new RabbitMqExchangeOptions
{
    Type = "topic",
    Durable = true,
    AutoDelete = true,
    Arguments = null,
    RequeueFailedMessages = true,
    DeadLetterExchange = "default.dlx.exchange",
    Queues = new List<RabbitMqQueueOptions>
    {
        new RabbitMqQueueOptions
        {
            Name = "MyQueueName",
            Durable = true,
            AutoDelete = false,
            Exclusive = false,
            Arguments = null,
            RoutingKeys = new HashSet<string> { "routing.key" }
        }
    }
};
services.AddRabbitMqClient(clientConfiguration)
     .AddExchange("ExchangeName", isConsuming: true, exchangeOptions);
 ```

The same configuration will work with `AddConsumptionExchange` or `AddProductionExchange` overloads.

```c#
var exchangeOptions = new RabbitMqExchangeOptions
{
    Queues = new List<RabbitMqQueueOptions>
    {
        new RabbitMqQueueOptions
        {
            Name = "MyQueueName",
            RoutingKeys = new HashSet<string> { "routing.key" }
        }
    }
};
services.AddRabbitMqClient(clientConfiguration)
    .AddProductionExchange("ProductionExchange", exchangeOptions);

// Or the other method.

services.AddRabbitMqClient(clientConfiguration)
    .AddConsumptionExchange("ConsumptionExchange", exchangeConfiguration);
```

For the RabbitMQ client configuration see the [Previous page](rabbit-configuration.md) <br>
For message production features see the [Next page](message-production.md)