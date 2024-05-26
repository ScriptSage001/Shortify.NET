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
        private User(Guid id, UserName userName, Email email) : base(id)
        {
            this.UserName = userName;
            this.Email = email;
        }

        #endregion

        #region Properties

        public UserName UserName { get; private set; }

        public Email Email { get; private set; }

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }

        public bool RowStatus { get; set; }

        public IReadOnlyCollection<ShortenedUrl> ShortenedUrls => _shortenedUrls;

        public UserCredentials? UserCredentials { get; private set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// To Create New Users
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="mobileNumber"></param>
        /// <param name="altMobileNumber"></param>
        /// <returns></returns>
        public static User CreateUser(UserName userName, Email email)
        {
            var user = new User(Guid.NewGuid(), userName, email);

            //user.RaiseDomainEvent(new UserRegisteredDomainEvent(Guid.NewGuid(), user.Id, email.Value));

            return user;
        }

        #endregion
    }
}
