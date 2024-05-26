using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Applicaion.Abstractions
{
    public interface IAuthServices
    {
        /// <summary>
        /// To Create Password Hash
        /// </summary>
        /// <param name="password"></param>
        (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHashAndSalt(string password);

        // <summary>
        /// To Verify Password during Login
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns>boolean</returns>
        bool VerifyPasswordHash(string password, byte[] passwordhash, byte[] passwordSalt);

        /// <summary>
        /// To generate Authorization Bearer Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns>AuthenticationResult</returns>
        AuthenticationResult CreateToken(Guid userId, string userName, string email);

        /// <summary>
        /// To Refresh Authorization Bearer Token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="refreshToken"></param>
        /// <returns>AuthenticationResult</returns>
        Task<Result<AuthenticationResult>> RefreshToken(string accessToken, string refreshToken);

        /// <summary>
        /// To Generate Reset Password Token
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        string GenerateResetPasswordToken(string email);

        /// <summary>
        /// To Verify Reset Password Token
        /// </summary>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        bool VerifyResetPasswordToken(string email, string token);

        /// <summary>
        /// To Validate ClientSecret
        /// </summary>
        /// <param name="clientSecret"></param>
        /// <returns></returns>
        bool ValidateClientSecret(string clientSecret);
    }
}
