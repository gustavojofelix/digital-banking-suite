using System;
using System.Collections.Generic;
using System.Text;

namespace BankingSuite.IamService.Application.Common.Interfaces;

public interface ICurrentUser
{
    Guid UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}
