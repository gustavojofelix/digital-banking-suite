using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace BankingSuite.IamService.Application.Auth.Commands.ForgotPassword;

public sealed record ForgotPasswordCommand(string Email, string ResetBaseUrl) : IRequest<Unit>;
