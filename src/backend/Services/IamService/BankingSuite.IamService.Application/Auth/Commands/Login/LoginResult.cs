using System;
using System.Collections.Generic;
using System.Text;

namespace BankingSuite.IamService.Application.Auth.Commands.Login
;

public sealed class LoginResult
{
    // When false and AccessToken is set => normal login success.
    // When true => 2FA still pending; AccessToken is null.
    public bool RequiresTwoFactor { get; init; }

    // Used by the frontend to call the 2FA verify endpoint.
    public Guid? UserId { get; init; }

    // Populated only when 2FA is not required or already completed.
    public string? AccessToken { get; init; }

    public DateTime? ExpiresAt { get; init; }

    // Optional: add RefreshToken, roles, etc if your app uses them.
}
