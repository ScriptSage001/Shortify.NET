namespace Shortify.NET.Application.Shared.Models
{
    /// <summary>
    /// Shared Record for MailRequest 
    /// </summary>
    /// <param name="Recipients"></param>
    /// <param name="Subject"></param>
    /// <param name="Body"></param>
    public sealed record MailRequest(
        List<string> Recipients,
        string Subject,
        string Body
    );
}
