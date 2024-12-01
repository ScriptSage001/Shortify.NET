using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Users.Queries.IsEmailUnique;

public record IsEmailUniqueQuery(string Email) : IQuery<bool>;