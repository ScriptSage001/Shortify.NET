using Shortify.NET.Application.Abstractions;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Token.Commands.RefreshToken
{
    internal sealed class RefreshTokenCommandHandler(IAuthServices authServices) 
        : ICommandHandler<RefreshTokenCommand, AuthenticationResult>
    {
        private readonly IAuthServices _authServices = authServices;

        public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authServices.RefreshToken(request.AccessToken, request.RefreshToken);
        }
    }
}
