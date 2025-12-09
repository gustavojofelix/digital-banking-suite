using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Token, string NewPassword)
    : IRequest<Unit>;
