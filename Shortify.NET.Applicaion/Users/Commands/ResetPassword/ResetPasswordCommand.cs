﻿using Shortify.NET.Common.Messaging.Abstractions;

namespace Shortify.NET.Applicaion.Users.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string UserId,
        string OldPassword,
        string NewPassword,
        string ConfirmPassword
        ) : ICommand;
}
