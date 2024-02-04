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
             .AddRebusConfigurationProviders((a =>
            {
                a.FileSystemTransport()  // allows "FileSystem" transport provider to be specified in config.
                    .InMemoryTransport(networkName => // allows "InMemory" transport provider to be specified in config.
                    {
                       // return a network for the given name specified in the config                    
                        return new InMemNetwork();
                    })
                    .ServiceBusTransport() // allows "ServiceBus" transport provider to be specified in config.
                    .SqlServerOutbox() // allows "SqlServer" outbox provider to be specified in config.
                    .UseConfigureCallback((context) =>
                    {
                        globalCallbackCalled = true;
                        return context.Configurer.Sagas(b => b.UseFilesystem("./foo"));
                    })
                    .UseConfigureCallback("Default", (context) =>
                    {
                        busSpecificCallbackCalled = true;
                        return context.Configurer;
                    })
            })).AddRebus(configuration.GetSection("Rebus"));            


```

Example appsettings.json - note the "FileSystem" provider is in use, the ServiceBus and InMemory provider sections are there as examples. See their full options below.

```json
{
  "Rebus": {
    "DefaultBus": "Default",
    "Buses": {
      "Default": {
        "Transport": {
          "QueueName": "manager",
          "ProviderName": "FileSystem",
          "Providers": {
            "FileSystem": {
              "BaseDirectory": "{PWD}/transport/"
            },
            "ServiceBus": {
              "AutomaticallyRenewPeekLock": true
            },
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

You can configure buses to use in memory networks, but you must provider a factory function to return the network for a
given name specified in the config.

```csharp
 .UseInMemoryTransportProvider(networkName =>
                {
                    // return a network for the given name specified in the config
                    return new InMemNetwork();
                })
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
          "ProviderName": "FileSystem", 
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
          "ProviderName": "FileSystem", 
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

