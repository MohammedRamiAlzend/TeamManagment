using TMS.Core.MediatR.Interfaces;

namespace TMS.Core.MediatR;

public class Sender(IServiceProvider serviceProvider) : ISender
{

    public async Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var requestType = request.GetType();
        var handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, typeof(TResponse));

        dynamic handler = serviceProvider.GetService(handlerType);
        return handler == null
            ? throw new InvalidOperationException($"No handler registered for request type {requestType.Name}")
            : (TResponse)await handler.Handle((dynamic)request, cancellationToken);
    }
}
