using MediatR;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Common.Messaging
{
    public class ApiService(IMediator mediator) : IApiService
    {
        private readonly IMediator _mediator = mediator;

        #region Request Methods

        public Task<Result<TResponse>> RequestAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(query));

            return _mediator.Send(query, cancellationToken);
        }

        public Result<TResponse> Request<TResponse>(IQuery<TResponse> query)
        {
            ArgumentNullException.ThrowIfNull(nameof(query));

            return _mediator.Send(query).Result;
        }

        #endregion

        #region Send Methods

        public Result Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            ArgumentNullException.ThrowIfNull(nameof(command));

            return _mediator.Send(command).Result;
        }

        public Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand
        {
            ArgumentNullException.ThrowIfNull(nameof(command));

            return _mediator.Send(command, cancellationToken);
        }

        public Result<TResponse> Send<TResponse>(ICommand<TResponse> command)
        {
            ArgumentNullException.ThrowIfNull(nameof(command));

            return _mediator.Send(command).Result;
        }

        public Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(nameof(command));

            return _mediator.Send(command, cancellationToken);
        }

        #endregion

        #region Publisher Methods

        public async Task EventPublisher<TEvent>(TEvent eventMessage, CancellationToken cancellationToken = default)
            where TEvent : IEvent
        {
            ArgumentNullException.ThrowIfNull(nameof(eventMessage));
            await _mediator.Publish(eventMessage, cancellationToken);
        }

        #endregion
    }
}
