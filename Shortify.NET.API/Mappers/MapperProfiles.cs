using Riok.Mapperly.Abstractions;
using Shortify.NET.API.Contracts;
using Shortify.NET.Applicaion.Otp.Commands.LoginUsingOtp;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Applicaion.Users.Commands.ForgetPassword;
using Shortify.NET.Applicaion.Users.Commands.LoginUser;
using Shortify.NET.Applicaion.Users.Commands.RegisterUser;
using Shortify.NET.Applicaion.Users.Commands.ResetPassword;

namespace Shortify.NET.API.Mappers
{
    public partial interface IMapperProfiles
    {
        ResetPasswordCommand ResetPasswordRequestToCommand(ResetPasswordRequest request, string userId);
    }

    [Mapper]
    public partial class MapperProfiles : IMapperProfiles
    {
        public partial RegisterUserCommand RegisterUserRequestToCommand(RegisterUserRequest request);
        
        public partial RegisterUserResponse AuthResultToRegisterUserResponse(AuthenticationResult result);
        
        public partial LoginUserCommand LoginUserRequestToCommand(LoginUserRequest request);

        public partial LoginUsingOtpCommand LoginUsingOtpRequestToCommand(LoginUsingOtpRequest request);
        
        public partial LoginUserResponse AuthResultToLoginUserResponse(AuthenticationResult result);

        public partial UserResponse UserDtoToUserResponse(UserDto userDto);
        

        public ResetPasswordCommand ResetPasswordRequestToCommand(ResetPasswordRequest request, string userId)
        {
            return new ResetPasswordCommand(
                            UserId: userId,
                            OldPassword: request.OldPassword,
                            NewPassword: request.NewPassword,
                            ConfirmPassword: request.ConfirmPassword);
        }

        public partial ResetPasswordUsingOtpCommand ResetPasswordUsingOtpRequestToCommand(ResetPasswordUsingOtpRequest request);
    }
}
