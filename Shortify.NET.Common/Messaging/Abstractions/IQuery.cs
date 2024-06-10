using MediatR;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    public interface IQuery<TResponse> : IRequest<Result<TResponse>>;
}
