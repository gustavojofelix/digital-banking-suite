using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.ChangePassword;

public sealed record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<Unit>;
