namespace Shortify.NET.API.Contracts
{
    public sealed record LoginUserRequest(
        string UserName,
        string Password);
}
