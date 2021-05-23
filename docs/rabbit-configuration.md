# RabbitMQ configuration

## Basic configuration

In order to connect to a RabbitMQ server, it is necessary to register services that library provides and configure them to use an endpoint, credentials or other valuable connection settings. It can be done with the extension method `AddRabbitMqServices` that can be used with `IServiceCollection` inside the `Startup` class.

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
        services.AddRabbitMqServices(Configuration.GetSection("RabbitMq"));
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
    }
}
```








The `AddRabbitMqServices` method will add an `IQueueService` as a **singleton**, but you can register it in the **transient** mode simply calling the `AddRabbitMqClientTransient` method which takes the same set of parameters.

A RabbitMQ client can be configured via a configuration section located in the `appsettings.json` file. This configuration section must be of a certain format and down below is an example of all configuration options used in `IQueueService`.

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
    "RequestedHeartbeat": 60,
    "InitialConnectionRetries": 5,
    "InitialConnectionRetryTimeoutMilliseconds": 200
  }
}
```

A RabbitMQ connection can be configured with properties:
- `HostName` - RabbitMQ server,
- `HostNames` - collection of RabbitMQ hostnames,
- `TcpEndpoints` - collection of AMPQ TCP endpoints,
- `Port` - port RabbitMQ running on,
- `UserName` - user that connects to the server,
- `Password` - password of the user that connects to the server,
- `ClientProvidedName` - application-specific connection name that will be displayed in the management UI if RabbitMQ server supports it,
- `VirtualHost` - the default virtual host,
- `AutomaticRecoveryEnabled` - automatic connection recovery option,
- `TopologyRecoveryEnabled` - topology recovery option,
- `RequestedConnectionTimeout` - timeout for connection attempts,
- `RequestedHeartbeat` - heartbeat timeout,
- `InitialConnectionRetries` - a number of retries which could be attempted while trying to make an initial connection,
- `InitialConnectionRetryTimeoutMilliseconds` - timeout in milliseconds which could be used while trying to make an initial connection.

`ClientProvidedName` is optional and can be null. Options `VirtualHost`, `AutomaticRecoveryEnabled`, `TopologyRecoveryEnabled`, `RequestedConnectionTimeout`, `RequestedHeartbeat`, `InitialConnectionRetries`, `InitialConnectionRetryTimeoutMilliseconds` are set with default values, so you can leave them.

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

For high availability RabbitMQ clusters with multiple nodes you can set hosts collection with the `HostNames` option.

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

## Multiple nodes configuration

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

There is an importance of `TcpEndpoints`, `HostNames`, `HostName` options. If all of them are set connection will be created using the `TcpEndpoints` option. The second will be the option `HostNames` and the `HostName` option is the least important between those three.

If `.json` configuration files unacceptable for you there is another `AddRabbitMqClient` extension method that takes manually created `RabbitMqClientOptions` configuration as a parameter. `RabbitMqClientOptions` is the model class used for injecting a RabbitMQ client configuration as an `IOtions<T>` instance so properties named the same as in the previous method.

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

There is also the `AddRabbitMqClientTransient` method which takes `RabbitMqClientOptions`.

## Ssl configuration

In case you want to establish an ssl connection you can use advanced `TcpEndpoints` configuration.
`RabbitMqTcpEndpoint` has following properties:
- `HostName` - RabbitMQ server.
- `Port` - tcp connection port. The default ssl port is 5671.
- `SslOption` - ssl options model `RabbitMqSslOption`.

Ssl options model `RabbitMqSslOption` consists of:
- `ServerName` - canonical server name (CA).
- `CertificatePath` - path to your certificate (a key store).
- `CertificatePassphrase` - passphrase for client certificate.
- `Enabled` - flag that defines if certificate should be used. The default value is true.
- `AcceptablePolicyErrors` - acceptable policy errors. Flags enum `SslPolicyErrors`. The default value is null.

The key part is to make a proper configuration - right `ServerName`, `CertificatePath` (if needed) and `AcceptablePolicyErrors`.

```c#
var rabbitMqConfiguration = new RabbitMqClientOptions
{
    UserName = "guest",
    Password = "guest",
    TcpEndpoints = new List<RabbitMqTcpEndpoint>
    {
        new RabbitMqTcpEndpoint
        {
            HostName = "127.0.0.1",
            Port = 5671,
            SslOption = new RabbitMqSslOption
            {
                Enabled = true,
                ServerName = "yourCA",
                CertificatePath = "/path/tp/client-key-store.p12",
                CertificatePassphrase = "yourPathPhrase",
                AcceptablePolicyErrors = SslPolicyErrors.RemoteCertificateChainErrors | SslPolicyErrors.RemoteCertificateNameMismatch
            }
        }
    }
};
```

The same configuration can be in appsettings.json file.

```json
{
    "RabbitMq": {
        "TcpEndpoints": [
            {
                "HostName": "127.0.0.1",
                "Port": 5671,
                "SslOption": {
                    "Enabled": true,
                    "ServerName": "yourCA",
                    "CertificatePath": "/path/tp/client-key-store.p12",
                    "CertificatePassphrase": "yourPathPhrase",
                    "AcceptablePolicyErrors": "RemoteCertificateChainErrors, RemoteCertificateNameMismatch"
                }
            }
        ],
        "UserName": "guest",
        "Password": "guest"
    }
}
```

Map that configuration using standard `GetSection` method.

```c#
var rabbitMqConfiguration = Configuration.GetSection("RabbitMq");
```

Pass it to the `AddRabbitMqClient` extension method as always.

```c#
services.AddRabbitMqClient(rabbitMqConfiguration);
```

For the exchange configuration see the [Next page](exchange-configuration.md)