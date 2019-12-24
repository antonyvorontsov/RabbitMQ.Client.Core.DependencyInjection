# RabbitMQ configuration

To connect to a RabbitMQ, it is necessary to instantiate an `IQueueService` and configure it to use desired endpoint, credentials and other valuable connection settings.
`IQueueService` allows clients to configure queues to exchange bindings, and to consume and produce messages in different ways (sync or async, with or without delay). To add `IQueueService` in your application simply use `AddRabbitMqClient` extension method as in the example below.

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
        services.AddRabbitMqClient(Configuration.GetSection("RabbitMq"));
    }
    
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
    }
}
```

RabbitMQ client can be configured via configuration section located in the `appsettings.json` file. This configuration section must be of a certain format and down below is an example of all configuration options used in `IQueueService`.

```json
{
 "RabbitMq": {
    "HostName": "127.0.0.1",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest",
    "ClientProvidedName": "Custom connection name",
    "VirtualHost": "/",
    "AutomaticRecoveryEnabled": true,
    "TopologyRecoveryEnabled": true,
    "RequestedConnectionTimeout": 60000,
    "RequestedHeartbeat": 60
  }
}
```

RabbitMQ connection can be configured with properties:
- `HostName`  - RabbitMQ server,
- `HostNames` - collection of RabbitMQ hostnames,
- `TcpEndpoints` - collection of AMPQ TCP endpoints,
- `Port` - port RabbitMQ running on,
- `UserName` - user that connects to the server,
- `Password` - password of the user that connects to the server,
- `ClientProvidedName` - application-specific connection name that will be displayed in the management UI if RabbitMQ server supports it,
- `VirtualHost` - default virtual host,
- `AutomaticRecoveryEnabled` - automatic connection recovery option,
- `TopologyRecoveryEnabled` - topology recovery option,
- `RequestedConnectionTimeout` - timeout for connection attempts,
- `RequestedHeartbeat` - heartbeat timeout.

`ClientProvidedName` is optional and can be null. Options `VirtualHost`, `AutomaticRecoveryEnabled`, `TopologyRecoveryEnabled`, `RequestedConnectionTimeout`, `RequestedHeartbeat` are set with default values, so you can leave them.

```json
{
 "RabbitMq": {
    "HostName": "127.0.0.1",
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

For high availability RabbitMQ clusters with multiple nodes you can set hosts collection with `HostNames` option.

```json
{
 "RabbitMq": {
    "HostNames": [ "hostname1", "hostname2", "hostname3" ],
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

If nodes are running on different hosts with different ports you have an option of configuring that via `TcpEndpoints`.

```json
{
 "RabbitMq": {
    "TcpEndpoints": [
      {
        "HostName": "hostname1",
        "Port": 5672
      },
      {
        "HostName": "hostname2",
        "Port": 45672
      },
      {
        "HostName": "hostname3",
        "Port": 345672
      }
    ],
    "Port": "5672",
    "UserName": "guest",
    "Password": "guest"
  }
}
```

There is an importance of `TcpEndpoints`, `HostNames`, `HostName` options. If all of them are set connection will be created using `TcpEndpoints` option. The second will be option `HostNames` and the `HostName` option is the least important between those three.

If `.json` configuration files unacceptable for you there is another `AddRabbitMqClient` extension method that takes manually created `RabbitMqClientOptions` configuration as a parameter. `RabbitMqClientOptions` is the model class used for injecting RabbitMQ client configuration as an `IOtions<T>` instance so properties named the same as in the previous method.

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
        var configuration = new RabbitMqClientOptions
        {
           HostName = "localhost",
           Port = 5432,
           UserName = "guest",
           Password = "guest"
        };
        services.AddRabbitMqClient(configuration);
    }
    
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
    }
}
```

For the exchange configuration see the [Next page](exchange-configuration.md)