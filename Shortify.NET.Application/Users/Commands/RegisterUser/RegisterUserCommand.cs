﻿using Shortify.NET.Application.Shared.Models;
using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Application.Users.Commands.RegisterUser
{
    public sealed record RegisterUserCommand(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string ValidateOtpToken,
        string UserRole
        ) : ICommand<AuthenticationResult>;
}
