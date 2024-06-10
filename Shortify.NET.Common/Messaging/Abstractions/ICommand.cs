using MediatR;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    public interface ICommand : IRequest<Result>;

    public interface ICommand<TResponse> : IRequest<Result<TResponse>>;
}
