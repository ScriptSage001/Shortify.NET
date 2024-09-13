using Shortify.NET.Core.Primitives;

namespace Shortify.NET.Core.Entites
{
    /// <summary>
    /// Represents the UserRole Entity
    /// </summary>
    public class UserRole : IAuditable
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of UserRole
        /// </summary>
        /// <param name="userId">User Entity Identifier of type Guid</param>
        /// <param name="roleId">Role Entity Identifier of type int</param>
        private UserRole(
            Guid userId, 
            int roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }

        #endregion
        
        #region Properties
        
        public Guid UserId { get; private set; }
        public int RoleId { get; private set; }
        public DateTime CreatedOnUtc { get; set; }
        public DateTime? UpdatedOnUtc { get; set; }
        public bool RowStatus { get; set; }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Factory Method to Create a new instance of UserRole
        /// </summary>
        /// <param name="userId">User Entity Guid Identifier</param>
        /// <param name="roleId">Role Entity int Identifier</param>
        /// <returns></returns>
        public static UserRole Create(Guid userId, int roleId)
        {
            return new UserRole(userId, roleId);
        }

        #endregion
    }
}