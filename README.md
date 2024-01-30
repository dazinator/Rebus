## Rebus Extensions

Add `Rebus` to your application and configure it purely from `IConfiguration`.

The scope of what can be configured (more being added):-

- A single or multiple buses, including which is the "default"
    - Transport
        - In Memory
        - File System
        - Service Bus
    - Outbox
        - Sql Server

```csharp

        configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();


        var services = new ServiceCollection()
            .AddRebusFromConfiguration(configuration.GetSection("Rebus"), a =>
        {
            a.UseFileSystemTransportProvider()
                .UseInMemoryTransportProvider()
                .UseServiceBusTransportProvider()
                .UseSqlServerOutboxProvider();
        });


```

## appsettings.json examples

### Service Bus Transport

```json

{
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Transport": {
          "QueueName": "test",
          "ProviderName": "ServiceBus",
          "Providers": {
            "ServiceBus": {
              "ConnectionString": "sb://foo.bar",
              "ConnectionStringAccessKey": "hadwa",
              "EnablePartitioning": false,
              "MessagePayloadSizeLimitInBytes": 50000000,
              "DuplicateDetectionHistoryTimeWindow": "00:10:00",
              "AutoDeleteOnIdle": "01:00:00",
              "DefaultMessageTimeToLive": "00:01:00",
              "MessagePeekLockDuration": "00:02:00",
              "NumberOfMessagesToPrefetch": 1,
              "AutomaticallyRenewPeekLock": true,
              "UseLegacyNaming": false,
              "AutoCreateQueues": true,
              "CheckQueueConfiguration": true,
              "ReceiveOperationTimeout": "00:01:00"
            }
          }
        }
      }
    }
  }
}
```

### In Memory Transport

You can configure buses to use in memory networks, but you first have to register a func to return the network for a
given name which can then be used in the config.

```csharp
var services = new ServiceCollection().AddSingleton<Func<string, InMemNetwork>>((sp) => (name) =>
{
  var network = new InMemNetwork();
  return network;
});
```

```json
{
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Transport": {
          "QueueName": "test",
          "ProviderName": "InMemory",
          "Providers": {
            "InMemory": {
              "NetworkName": "Test",
              "RegisterForSubscriptionStorage": true
            }
          }
        }
      }
    }
  }
}
```

### File System Transport

```json
{
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Transport": {
          "QueueName": "test",
          "ProviderName": "FileSystem",
          "Providers": {
            "FileSystem": {
              "BaseDirectory": "c://foo/bar",
              "Prefetch": 10
            }
          }
        }
      }
    }
  }
}
```

### Multiple buses:-

```json

{  
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Transport": {
          "QueueName": "test",
          "ProviderName": "FileSystem", // InMemory, ServiceBus etc
          "Providers": {
            "FileSystem": {
              "BaseDirectory": "{PWD}/transport/"
            }
          }
        }
      },
      "Another": {
        "Transport": {
          "QueueName": "another",
          "ProviderName": "FileSystem", // InMemory, ServiceBus etc
          "Providers": {
            "FileSystem": {
              "BaseDirectory": "{PWD}/transport/"
            }
          }
        }
      }
    }
  }
}

```

### Outbox

Can be configured for any of the buses, rest of configuration ommitted for brevity.

#### SQL Server

```json
{
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Outbox": {
          "ProviderName": "SqlServer",
          "Providers": {
            "SqlServer": {
              "ConnectionString": "Server=.;Database=Rebus;Trusted_Connection=True;",
              "TableName": "Outbox"
            }
          }
        }
      }
    }
  }
}
```

