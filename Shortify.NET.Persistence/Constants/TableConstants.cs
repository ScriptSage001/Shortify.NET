﻿using Shortify.NET.Core.Entites;

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
        }
    }
}