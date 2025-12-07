using BankingSuite.IamService.Application.Employees.Commands;
using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.Application.Employees.Commands.ActivateEmployee;

public sealed class ActivateEmployeeCommandHandler : IRequestHandler<ActivateEmployeeCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivateEmployeeCommandHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Unit> Handle(ActivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException("Employee not found.");

        user.IsActive = true;
        user.LockoutEnd = null;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to activate employee: {errors}");
        }

        return Unit.Value;
    }
}
