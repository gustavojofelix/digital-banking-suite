using System;
using System.Collections.Generic;
using System.Text;
using BankingSuite.IamService.Application.Auth.Commands.Login;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.VerifyTwoFactor;

public sealed record VerifyTwoFactorCommand(Guid UserId, string Code) : IRequest<LoginResult>;
