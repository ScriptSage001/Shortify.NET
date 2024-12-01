namespace Shortify.NET.Application.Shared.Models
{
    /// <summary>
    /// DTO of User
    /// </summary>
    public record UserDto(
        Guid Id,
        string UserName,
        string Email,
        DateTime CreatedOnUtc,
        DateTime? UpdatedOnUtc,
        bool RowStatus
        );
}
