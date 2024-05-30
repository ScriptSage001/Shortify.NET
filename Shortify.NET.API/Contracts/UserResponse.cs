namespace Shortify.NET.API.Contracts
{
    public sealed record UserResponse(
        Guid Id,
        string UserName,
        string Email,
        DateTime CreatedOnUtc,
        DateTime? UpdatedOnUtc,
        bool RowStatus
        );
}
