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
    public sealed class AuthServices : IAuthServices
    {
        private readonly AppSettings _appSettings;

        private readonly IUserCredentialsRepository _userCredentialsRepository;

        private readonly IUnitOfWork _unitOfWork;

        public AuthServices(IOptions<AppSettings> appSettings, IUserCredentialsRepository userCredentialsRepository, IUnitOfWork unitOfWork)
        {
            _appSettings = appSettings.Value;
            _userCredentialsRepository = userCredentialsRepository;
            _unitOfWork = unitOfWork;
        }

        #region Public Methods

        /// <summary>
        /// Generates PasswordSalt and Hashed Password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public (byte[] passwordHash, byte[] passwordSalt) CreatePasswordHashAndSalt(string password)
        {
            byte[] salt;
            byte[] hash;

            using (var hmac = new HMACSHA512())
            {
                salt = hmac.Key;
                hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return (hash, salt);
        }

        /// <summary>
        /// Creates Access and Refresh Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
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
                Subject = new ClaimsIdentity(claims.ToArray()),
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

        public bool ValidateClientSecret(string clientSecret)
        {
            return _appSettings.ClientSecret.Equals(clientSecret);
        }

        /// <summary>
        /// To Verify Password 
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            byte[] newHash;

            using (var hmac = new HMACSHA512(passwordSalt))
            {
                newHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }

            return newHash.SequenceEqual(passwordHash);
        }

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
        /// Generates JWT Access Token
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <returns></returns>
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
        /// Generates Refresh Token
        /// </summary>
        /// <param name="expirationTime"></param>
        /// <returns></returns>
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
