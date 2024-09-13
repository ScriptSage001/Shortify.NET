namespace Shortify.NET.Core.Entites
{
    /// <summary>
    /// Represents the Role Entity of MasterData
    /// </summary>
    public class Role
    {
        #region Constructor
        
        /// <summary>
        /// Initializes a new instance of Role
        /// </summary>
        /// <param name="id">Role Entity Identifier of type INT</param>
        /// <param name="name">Role Name</param>
        public Role(int id, string name)
        {
            Id = id;
            Name = name;
        }

        #endregion

        #region Properties

        public int Id { get; private set; }

        public string Name { get; private set; }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Factory Method to Create a new instance of UserRole
        /// </summary>
        /// <param name="id">Int Identifier for Role</param>
        /// <param name="name">Name of the Role</param>
        /// <returns></returns>
        public static Role Create(int id, string name)
        {
            return new Role(id, name);
        }

        #endregion
    }
}