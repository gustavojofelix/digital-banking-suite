using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.TwoFactor;
public sealed record EnableTwoFactorCommand(string CurrentPassword) : IRequest<Unit>;
