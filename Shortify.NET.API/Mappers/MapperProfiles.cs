using Riok.Mapperly.Abstractions;
using Shortify.NET.API.Contracts;
using Shortify.NET.Application.Otp.Commands.LoginUsingOtp;
using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Application.Token.Commands.GetTokenByClientSecret;
using Shortify.NET.Application.Token.Commands.RefreshToken;
using Shortify.NET.Application.Url.Commands.ShortenUrl;
using Shortify.NET.Application.Url.Commands.UpdateUrl;
using Shortify.NET.Application.Users.Commands.ForgetPassword;
using Shortify.NET.Application.Users.Commands.LoginUser;
using Shortify.NET.Application.Users.Commands.RegisterUser;
using Shortify.NET.Application.Users.Commands.ResetPassword;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.API.Mappers
{
    [Mapper]
    public partial class MapperProfiles
    {
        public RegisterUserCommand RegisterUserRequestToCommand(RegisterUserRequest request, string role)
        {
            return new RegisterUserCommand(
                UserName: request.UserName,
                Email: request.Email,
                Password: request.Password,
                ConfirmPassword: request.ConfirmPassword,
                ValidateOtpToken: request.ValidateOtpToken,
                UserRole: role);
        }
        
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

        public partial RefreshTokenCommand RefreshTokenRequestToCommand(RefreshTokenRequest request);

        public partial AuthenticationResponse AuthenticationResultToResponse(AuthenticationResult result);

        public partial GenerateTokenByClientSecretCommand ClientCredentialsToGenerateTokenByClientSecretCommand(ClientCredentials clientCredentials);

        public ShortenUrlCommand ShortenUrlRequestToCommand(ShortenUrlRequest request, string userId, HttpRequest httpRequest)
        {
            return new ShortenUrlCommand(
                            Url: request.Url,
                            UserId: userId,
                            Title: request.Title,
                            Tags: request.Tags,
                            HttpRequest: httpRequest);
        }

        public partial ShortenedUrlResponse ShortenedUrlDtoToResponse(ShortenedUrlDto dtos);

        public partial PagedList<ShortenedUrlResponse> ShortenedUrlDtoListToResponseList(PagedList<ShortenedUrlDto> dtos);

        public partial UpdateShortenedUrlCommand UpdateShortenedUrlRequestToCommand(UpdateShortenedUrlRequest request);
    }
}
