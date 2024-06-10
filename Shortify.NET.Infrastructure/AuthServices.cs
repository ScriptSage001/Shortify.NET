using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Applicaion.Shared.Models;
using Shortify.NET.Common.FunctionalTypes;
using Shortify.NET.Core;
using Shortify.NET.Infrastructure.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using static Shortify.NET.Infrastructure.Constants.AuthConstants;
using static Shortify.NET.Infrastructure.InfraErrors;

namespace Shortify.NET.Infrastructure
{
    /// <summary>
    /// Implements the IAuthServices interface, providing methods for user authentication, token generation, and password management.
    /// </summary>
    /// <param name = "appSettings" > The application settings containing configuration values.</param>
    /// <param name = "userCredentialsRepository" > The repository for accessing user credentials data.</param>
    /// <param name = "unitOfWork" > The unit of work for managing database transactions.</param>
    public sealed class AuthServices(
        IOptions<AppSettings> appSettings, 
        IUserCredentialsRepository userCredentialsRepository, 
        IUnitOfWork unitOfWork) 
        : IAuthServices
    {
        private readonly AppSettings _appSettings = appSettings.Value;

        private readonly IUserCredentialsRepository _userCredentialsRepository = userCredentialsRepository;

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        #region Public Methods

        /// <summary>
        /// Generates a password hash and salt for the provided password.
        /// </summary>
        /// <param name="password">The plaintext password to hash.</param>
        /// <returns>A tuple containing the password hash and salt.</returns>
        public (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHashAndSalt(string password)
        {
            using var hmac = new HMACSHA512();
            return (hmac.ComputeHash(Encoding.UTF8.GetBytes(password)), hmac.Key);
        }

        /// <summary>
        /// Creates access and refresh tokens for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>An AuthenticationResult containing the generated tokens and their expiration times.</returns>
        public AuthenticationResult CreateToken(Guid userId, string userName, string email)
        {
            string accessToken = GenerateAccessToken(userId, userName, email);
            string refreshToken = GenerateRefreshToken(out DateTime refreshTokenExpirationTime);

            return new AuthenticationResult
            (
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                RefreshTokenExpirationTimeUtc: refreshTokenExpirationTime,
                UserId: userId
            );
        }

        /// <summary>
        /// Generates a token for validating an OTP (One-Time Password) for the specified email.
        /// </summary>
        /// <param name="email">The email address for which the OTP token is generated.</param>
        /// <returns>A JWT token for OTP validation.</returns>
        public string GenerateValidateOtpToken(string email)
        {
            List<Claim> claims =
            [
                new Claim(ClaimType.Email, email),
                new Claim(ClaimType.TokenType, ClaimTypeValue.ValidateOtp)
            ];

            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            var tokenExpirationTime = _appSettings.ValidateOtpTokenExpirationTimeInMin;

            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _appSettings.Issuer,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTime == 0 ? 5 : tokenExpirationTime),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        /// <summary>
        /// Refreshes the access and refresh tokens using the provided tokens.
        /// </summary>
        /// <param name="accessToken">The current access token.</param>
        /// <param name="refreshToken">The current refresh token.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a Result object with the new AuthenticationResult.</returns>
        public async Task<Result<AuthenticationResult>> RefreshToken(string accessToken, string refreshToken)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(accessToken);

            string userIdFromClaims = token.Claims.First(c => c.Type == ClaimType.UserId).Value;
            string userNameFromClaims = token.Claims.First(c => c.Type == ClaimType.UserName).Value;
            string emailFromClaims = token.Claims.First(c => c.Type == ClaimType.Email).Value;

            Guid userId = Guid.Parse(userIdFromClaims);

            var userCredentials = await _userCredentialsRepository.GetByUserIdAsync(userId);

            if (userCredentials is null ||
                userCredentials.RefreshToken != refreshToken ||
                userCredentials.RefreshTokenExpirationTimeUtc < DateTime.UtcNow)
            {
                return Result.Failure<AuthenticationResult>(Auth.InvalidCredentials);
            }
            else
            {
                string newAccessToken = GenerateAccessToken(userId, userNameFromClaims, emailFromClaims);
                string newRefreshToken = GenerateRefreshToken(out DateTime expirationTime);

                userCredentials.AddOrUpdateRefreshToken(newRefreshToken, expirationTime);

                _userCredentialsRepository.Update(userCredentials);
                await _unitOfWork.SaveChangesAsync();

                return new AuthenticationResult(
                                UserId: userId,
                                AccessToken: newAccessToken,
                                RefreshToken: newRefreshToken,
                                RefreshTokenExpirationTimeUtc: expirationTime);
            }
        }

        /// <summary>
        /// Validates the provided client secret against the stored client secret.
        /// </summary>
        /// <param name="clientSecret">The client secret to validate.</param>
        /// <returns>
        /// True if the client secret is valid, otherwise false.
        /// </returns>
        public bool ValidateClientSecret(string clientSecret)
        {
            return _appSettings.ClientSecret.Equals(clientSecret);
        }

        /// <summary>
        /// Verifies that the provided password matches the stored password hash and salt.
        /// </summary>
        /// <param name="password">The plaintext password to verify.</param>
        /// <param name="passwordHash">The stored password hash.</param>
        /// <param name="passwordSalt">The stored password salt.</param>
        /// <returns>
        /// True if the password matches the hash, otherwise false.
        /// </returns>
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return computedHash.SequenceEqual(passwordHash);
        }

        /// <summary>
        /// Verifies the provided OTP token for the specified email.
        /// </summary>
        /// <param name="email">The email address to verify the token against.</param>
        /// <param name="token">The OTP token to verify.</param>
        /// <returns>
        /// True if the token is valid and not expired, otherwise false.
        /// </returns>
        public bool VerifyValidateOtpToken(string email, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            var userEmail = jwtToken.Claims.First(c => c.Type == ClaimType.Email)?.Value;
            var tokenType = jwtToken.Claims.First(c => c.Type == ClaimType.TokenType)?.Value;
            var expiresOn = jwtToken.ValidTo;

            return (email.Equals(userEmail, StringComparison.OrdinalIgnoreCase)
                    && ClaimTypeValue.ValidateOtp.Equals(tokenType, StringComparison.OrdinalIgnoreCase)
                    && expiresOn >= DateTime.UtcNow);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Generates a JWT access token for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="userName">The username of the user.</param>
        /// <param name="email">The email of the user.</param>
        /// <returns>
        /// A JWT access token.
        /// </returns>
        private string GenerateAccessToken(Guid userId, string userName, string email)
        {
            List<Claim> claims =
            [
                new Claim(ClaimType.UserId, userId.ToString()),
                new Claim(ClaimType.UserName, userName),
                new Claim(ClaimType.Email, email),
                new Claim(ClaimType.TokenType, ClaimTypeValue.AccessToken)
            ];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            int tokenExpirationTime = _appSettings.TokenExpirationTime;

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims.ToArray()),
                Issuer = _appSettings.Issuer,
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(tokenExpirationTime == 0 ? 1440 : tokenExpirationTime),
                SigningCredentials = credentials
            };

            JwtSecurityTokenHandler tokenHandler = new();
            SecurityToken token = tokenHandler.CreateToken(tokenDescriptor);
            string jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        /// <summary>
        /// Generates a refresh token and sets its expiration time.
        /// </summary>
        /// <param name="expirationTime">The expiration time of the refresh token.</param>
        /// <returns>
        /// A refresh token.
        /// </returns>
        private string GenerateRefreshToken(out DateTime expirationTime)
        {
            expirationTime = DateTime.UtcNow.AddMinutes(
                                 _appSettings.RefreshTokenExpirationTimeInDays == 0 ?
                                    7 : _appSettings.RefreshTokenExpirationTimeInDays);

            var randomNumber = new byte[64];

            using (var randomGenerator = RandomNumberGenerator.Create())
            {
                randomGenerator.GetBytes(randomNumber);
            }

            return Convert.ToBase64String(randomNumber);
        }

        #endregion
    }
}
