using Shortify.NET.Core.Primitives;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Core.Entites
{
    public sealed class ShortenedUrl : Entity, IAuditable
    {
        #region Constructor

        /// <summary>
        /// Private Constructor for ShortenedUrl Entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="originalUrl"></param>
        /// <param name="shortUrl"></param>
        /// <param name="code"></param>
        /// <param name="title"></param>
        /// <param name="tags"></param>
        private ShortenedUrl(
            Guid id, 
            Guid userId, 
            string originalUrl, 
            ShortUrl shortUrl, 
            string code,
            string? title,
            List<string>? tags) 
            : base(id)
        {
            UserId = userId;
            OriginalUrl = originalUrl;
            ShortUrl = shortUrl;
            Code = code;
            Title = title;
            Tags = tags;
        }

        #endregion

        #region Properties

        public Guid UserId { get; private set; }

        public string OriginalUrl { get; private set; }

        public ShortUrl ShortUrl { get; private set; }

        public string Code { get; private set; }

        public string? Title { get; private set; }

        public List<string>? Tags { get; private set; }

        public DateTime CreatedOnUtc { get; set; }
        
        public DateTime? UpdatedOnUtc { get; set; }
        
        public bool RowStatus { get; set; }

        #endregion

        #region Public Methods

        public static ShortenedUrl Create(
            Guid userId, 
            string originalUrl, 
            ShortUrl shortUrl, 
            string code,
            string? title,
            List<string>? tags)
        {
            var shortenedUrl = new ShortenedUrl(
                                        Guid.NewGuid(), 
                                        userId, 
                                        originalUrl, 
                                        shortUrl, 
                                        code,
                                        title,
                                        tags);
            return shortenedUrl;
        }

        public void Update(
            string originalUrl,
            string? title,
            List<string>? tags)
        {
            this.OriginalUrl = originalUrl;
            this.Title = title;
            this.Tags = tags;
        }
        
        #endregion
    }
}
