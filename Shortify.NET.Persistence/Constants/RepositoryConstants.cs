namespace Shortify.NET.Persistence.Constants
{
    internal static class RepositoryConstants
    {
        internal struct SortConstants
        {
            internal struct SortProperty
            {
                internal const string Title = "title";
                internal const string CreatedOn = "createdon";
                internal const string UpdatedOn = "updatedon";
            }

            internal struct SortOrder
            {
                internal const string Descending = "desc";
            }
        }
    }
}
