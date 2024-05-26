namespace Shortify.NET.API.Contracts
{
    public sealed record RegisterUserRequest(
        string UserName,
        string Email,
        string Password);
}
