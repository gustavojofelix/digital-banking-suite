using BankingSuite.IamService.Application.Employees.Commands;
using BankingSuite.IamService.Application.Employees.Commands.ActivateEmployee;
using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.Application.Employees.Commands.DeactivateEmployee;

public sealed class DeactivateEmployeeCommandHandler : IRequestHandler<DeactivateEmployeeCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public DeactivateEmployeeCommandHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Unit> Handle(DeactivateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException("Employee not found.");

        user.IsActive = false;
        user.LockoutEnd = DateTimeOffset.UtcNow.AddYears(100); // effectively frozen

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to deactivate employee: {errors}");
        }

        return Unit.Value;
    }
}
