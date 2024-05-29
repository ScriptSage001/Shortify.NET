using Microsoft.Extensions.Options;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Infrastructure.Helpers;
using System.Text;

namespace Shortify.NET.Infrastructure
{
    public sealed class UrlShorteningService : IUrlShorteningService
    {
        private readonly ShortLinkSettings _shortLinkSettings;

        private readonly IShortenedUrlRepository _shortenedUrlRepository;

        public UrlShorteningService(
            IOptions<ShortLinkSettings> shortLinkSettings, 
            IShortenedUrlRepository shortenedUrlRepository)
        {
            _shortLinkSettings = shortLinkSettings.Value;
            _shortenedUrlRepository = shortenedUrlRepository;
        }

        /// <summary>
        /// Generates Code which are Unique across the application
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<string> GenerateUniqueCode(CancellationToken cancellationToken = default)
        {
            int codeLength = _shortLinkSettings.Length;
            string characters = _shortLinkSettings.CharacterRange;
            ulong baseValue = (ulong)characters.Length;

            while (true)
            {
                string code = EncodeBase62(codeLength, characters, baseValue);

                // Handling Edge Case
                // To check for Collison
                if (await _shortenedUrlRepository.IsCodeUniqueAsync(code, cancellationToken))
                {
                    return code;
                }
            }
        }

        /// <summary>
        /// Creates Unique Base62 Code using GUID
        /// </summary>
        /// <param name="codeLength"></param>
        /// <param name="characters"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static string EncodeBase62(int codeLength, string characters, ulong baseValue)
        {
            var guid = Guid.NewGuid();
            var bytes = guid.ToByteArray();
            var value = BitConverter.ToUInt64(bytes, 0);

            var sb = new StringBuilder();

            while (value > 0)
            {
                sb.Insert(0, characters[(int)(value % baseValue)]);
                value /= baseValue;
            }

            // Handling Edge Case
            // Where generated code length is less than the desired code length
            while(sb.Length < codeLength)
            {
                sb.Insert(0, characters[0]);
            }

            // Handling Edge Case
            // Where generated code length is more than the desired code length
            var code = sb.ToString().Substring(0, codeLength);

            return code;
        }
    }
}
