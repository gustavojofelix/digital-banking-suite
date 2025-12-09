using BankingSuite.IamService.Application.Employees.Commands;
using BankingSuite.IamService.Domain;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BankingSuite.IamService.Application.Employees.Commands.UpdateEmployee;

public sealed class UpdateEmployeeCommandHandler : IRequestHandler<UpdateEmployeeCommand, Unit>
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UpdateEmployeeCommandHandler(UserManager<ApplicationUser> userManager)
        => _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<Unit> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.EmployeeId, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException("Employee not found.");

        //if (request.FullName is not null)
        //    user.FullName = request.FullName;

        if (request.PhoneNumber is not null)
            user.PhoneNumber = request.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to update employee: {errors}");
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var requestedRoles = request.Roles ?? Array.Empty<string>();

        var rolesToAdd = requestedRoles.Except(currentRoles).ToArray();
        var rolesToRemove = currentRoles.Except(requestedRoles).ToArray();

        if (rolesToAdd.Any())
        {
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                var errors = string.Join("; ", addResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to add roles: {errors}");
            }
        }

        if (rolesToRemove.Any())
        {
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                var errors = string.Join("; ", removeResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Failed to remove roles: {errors}");
            }
        }

        return Unit.Value;
    }
}
