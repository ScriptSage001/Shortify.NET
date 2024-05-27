using Microsoft.Extensions.Options;
using Shortify.NET.Applicaion.Abstractions;
using Shortify.NET.Applicaion.Abstractions.Repositories;
using Shortify.NET.Infrastructure.Helpers;

namespace Shortify.NET.Infrastructure
{
    public sealed class UrlShorteningService : IUrlShorteningService
    {
        private readonly ShortLinkSettings _shortLinkSettings;

        private readonly IShortenedUrlRepository _shortenedUrlRepository;

        private readonly Random _random = new();

        public UrlShorteningService(
            IOptions<ShortLinkSettings> shortLinkSettings, 
            IShortenedUrlRepository shortenedUrlRepository)
        {
            _shortLinkSettings = shortLinkSettings.Value;
            _shortenedUrlRepository = shortenedUrlRepository;
        }

        public async Task<string> GenerateUniqueCode(CancellationToken cancellationToken = default)
        {
            var codeChars = new char[_shortLinkSettings.Length];
            int maxValue = _shortLinkSettings.CharacterRange.Length;

            while (true)
            {
                for (var i = 0; i < _shortLinkSettings.Length; i++)
                {
                    var randomIndex = _random.Next(maxValue);

                    codeChars[i] = _shortLinkSettings.CharacterRange[randomIndex];
                }

                var code = new string(codeChars);

                if (await _shortenedUrlRepository.IsCodeUniqueAsync(code, cancellationToken))
                {
                    return code;
                }
            }
        }
    }
}
