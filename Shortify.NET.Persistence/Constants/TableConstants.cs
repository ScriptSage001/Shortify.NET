namespace Shortify.NET.Persistence.Constants
{
    internal static class TableConstants
    {
        internal struct TableNames
        {
            internal const string Users = nameof(Users);

            internal const string UserCredentials = nameof(UserCredentials);

            internal const string ShortenedUrls = nameof(ShortenedUrls);

            internal const string OtpDetails = nameof(OtpDetails);

            internal const string OutboxMessage = nameof(OutboxMessage);

            internal const string OutboxMessageConsumer = nameof(OutboxMessageConsumer);
            
            internal const string Role = nameof(Role);
            
            internal const string UserRole = nameof(UserRole);
        }

        internal struct TableSchema
        {
            internal const string MasterData = "masterdata";
        }
    }
}
