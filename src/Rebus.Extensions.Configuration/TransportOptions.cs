﻿namespace Core.Infrastructure.Rebus;

public class TransportOptions
{
    public string ProviderName { get; set; } = string.Empty;

    public string QueueName { get; set; }
}
