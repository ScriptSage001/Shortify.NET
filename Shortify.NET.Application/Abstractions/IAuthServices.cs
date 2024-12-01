using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;

namespace Shortify.NET.Application.Abstractions
{
    public interface IAuthServices
    {
        /// <summary>
        /// Generates a password hash and salt for the provided password.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>A tuple containing the password hash and salt.</returns>
        (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHashAndSalt(string password);

        /// <summary>
        /// Verifies that the provided password matches the stored password hash and salt.
        /// </summary>
        /// <param name="password">The plaintext password to verify.</param>
        /// <param name="passwordHash">The stored password hash.</param>
        /// <param name="passwordSalt">The stored password salt.</param>
        /// <returns>
        /// True if the password matches the hash, otherwise false.
        /// </returns>
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);

        /// <summary>
        /// Creates access and refresh tokens for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <param name="roles">The Roles of the User</param>
        /// <returns>An AuthenticationResult containing the generated tokens and their expiration times.</returns>
        AuthenticationResult CreateToken(Guid userId, string userName, string email, List<string>  roles);

        /// <summary>
        /// Refreshes the access and refresh tokens using the provided tokens.
        /// </summary>
        /// <param name="accessToken">The current access token.</param>
        /// <param name="refreshToken">The current refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the new AuthenticationResult.</returns>
        Task<Result<AuthenticationResult>> RefreshToken(string accessToken, string refreshToken);

        /// <summary>
        /// Generates a token for validating an OTP (One-Time Password) for the specified email.
        /// </summary>
        /// <param name="email">The email address for which the OTP token is generated.</param>
        /// <returns>A JWT token for OTP validation.</returns>
        string GenerateValidateOtpToken(string email);

        /// <summary>
        /// Verifies the provided OTP token for the specified email.
        /// </summary>
        /// <param name="email">The email address to verify the token against.</param>
        /// <param name="token">The OTP token to verify.</param>
        /// <returns>
        /// True if the token is valid and not expired, otherwise false.
        /// </returns>
        bool VerifyValidateOtpToken(string email, string token);

        /// <summary>
        /// Validates the provided client secret against the stored client secret.
        /// </summary>
        /// <param name="clientSecret">The client secret to validate.</param>
        /// <returns>
        /// True if the client secret is valid, otherwise false.
        /// </returns>
        bool ValidateClientSecret(string clientSecret);
    }
}
