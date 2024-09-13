using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Url.Commands.DeleteUrl
{
    /// <summary>
    /// Represents a command to delete a shortened URL by its identifier.
    /// </summary>
    /// <param name="Id">The unique identifier of the shortened URL to be deleted.</param>
    public record DeleteShortenedUrlByIdCommand(Guid Id) : ICommand;
}