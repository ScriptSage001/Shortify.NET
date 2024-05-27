using Riok.Mapperly.Abstractions;
using Shortify.NET.API.Contracts;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Applicaion.Users.Commands.LoginUser;
using Shortify.NET.Applicaion.Users.Commands.RegisterUser;

namespace Shortify.NET.API.Mappers
{
    [Mapper]
    public partial class MapperProfiles
    {
        public partial RegisterUserCommand RegisterUserRequestToCommand(RegisterUserRequest request);
        public partial RegisterUserResponse AuthResultToRegisterUserResponse(AuthenticationResult result);
        public partial LoginUserCommand LoginUserRequestToCommand(LoginUserRequest request);
        public partial LoginUserResponse AuthResultToLoginUserResponse(AuthenticationResult result);
    }
}
