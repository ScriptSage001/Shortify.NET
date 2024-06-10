using MediatR;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Common.Messaging.Abstractions
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
        where TQuery : IQuery<TResponse>;
}
