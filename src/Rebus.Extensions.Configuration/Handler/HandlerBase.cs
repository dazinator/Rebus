namespace Rebus.Extensions.Configuration.Handler;

using Handlers;
using Pipeline;

// see https://github.com/rebus-org/Rebus/issues/776
public abstract class HandlerBase<TMessage> : IHandleMessages<TMessage>
{
    private readonly IMessageContext _messageContext;

    public HandlerBase(IMessageContext messageContext) => _messageContext = messageContext;


    public Task Handle(TMessage message)
    {
        var cancellationToken = _messageContext.GetCancellationToken();
        return HandleAsync(message, cancellationToken);
    }

    public abstract Task HandleAsync(TMessage message, CancellationToken cancellationToken);
}
