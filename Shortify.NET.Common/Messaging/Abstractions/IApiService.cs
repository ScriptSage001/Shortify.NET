using Shortify.NET.Common.FunctionalTypes;
using System.Windows.Input;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    /// <summary>
    /// Custom Mediator Service for Command Query Implementaiton
    /// </summary>
    public interface IApiService
    {
        #region Request Methods

        Task<Result<TResponse>> RequestAsync<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);

        Result<TResponse> Request<TResponse>(IQuery<TResponse> query);

        #endregion

        #region Send Methods

        Result Send<TCommand>(TCommand command) where TCommand : ICommand;

        Task<Result> SendAsync<TCommand>(TCommand command, CancellationToken cancellationToken = default) where TCommand : ICommand;

        Result<TResponse> Send<TResponse>(ICommand<TResponse> command);

        Task<Result<TResponse>> SendAsync<TResponse>(ICommand<TResponse> command, CancellationToken cancellationToken = default);

        #endregion

        #region Publisher Methods

        Task EventPublisher<TEvent>(TEvent eventMessage, CancellationToken cancellationToken = default) where TEvent : IEvent;

        #endregion
    }
}
