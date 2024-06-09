using Shortify.NET.Core.Events;
using Shortify.NET.Core.Primitives;
using Shortify.NET.Core.ValueObjects;

namespace Shortify.NET.Core.Entites
{
    public sealed class User : Entity, IAuditable
    {
        #region Feilds

        /// <summary>
        /// Private field for readonly Shortened Urls list
        /// </summary>
        private readonly List<ShortenedUrl> _shortenedUrls = [];

        #endregion

        #region Constructor

        /// <summary>
        /// Private Constructor for User Entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userName"></param>
        /// <param name="email"></param>
        /// <param name="userCredentials"></param>
        private User(
            Guid id, 
            UserName userName, 
            Email email,
            UserCredentials userCredentials) : base(id)
        {
            this.UserName = userName;
            this.Email = email;
            this.UserCredentials = userCredentials;
        }

        #endregion

        #region Properties

        public UserName UserName { get; private set; }

        public Email Email { get; private set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        public bool RowStatus { get; set; }

        public IReadOnlyCollection<ShortenedUrl> ShortenedUrls => _shortenedUrls;

        public UserCredentials UserCredentials { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates a new User with the specified username and email.
        /// </summary>
        /// <param name="id">The unique identifier of the new user.</param>
        /// <param name="userName">The username of the new user.</param>
        /// <param name="email">The email address of the new user.</param>
        /// <param name="userCredentials">The credentials of the new user.</param>
        /// <returns>A newly created User object.</returns>
        public static User CreateUser(
            Guid id, 
            UserName userName, 
            Email email,
            UserCredentials userCredentials)
        {
            var user = new User(id, userName, email, userCredentials);

            user.RaiseDomainEvent(
                    new UserRegisteredDomainEvent(
                            Guid.NewGuid(), user.Id));

            return user;
        }

        /// <summary>
        /// Sets the user credentials.
        /// </summary>
        /// <param name="credentials">The credentials to set for the user.</param>
        public void SetUserCredentials(UserCredentials credentials)
        {
            UserCredentials = credentials;
        }

        /// <summary>
        /// Adds a new shortened URL to the user's collection.
        /// </summary>
        /// <param name="shortenedUrl">The shortened URL to add.</param>
        public void AddShortenedUrl(ShortenedUrl shortenedUrl)
        {
            _shortenedUrls.Add(shortenedUrl);
        }

        #endregion
    }
}
