using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.ConfirmEmail;

public sealed record ConfirmEmailCommand(Guid UserId, string Token) : IRequest<Unit>;
