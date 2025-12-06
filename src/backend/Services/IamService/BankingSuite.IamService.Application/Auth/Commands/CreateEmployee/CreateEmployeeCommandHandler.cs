using BankingSuite.BuildingBlocks.Domain.Abstractions;
using BankingSuite.IamService.Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace BankingSuite.IamService.Application.Auth.Commands.CreateEmployee;

public sealed class CreateEmployeeCommandHandler
    : IRequestHandler<CreateEmployeeCommand, Result<CreateEmployeeResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateEmployeeCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager
    )
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<Result<CreateEmployeeResult>> Handle(
        CreateEmployeeCommand request,
        CancellationToken ct
    )
    {
        if (
            string.IsNullOrWhiteSpace(request.Email)
            || string.IsNullOrWhiteSpace(request.Password)
            || string.IsNullOrWhiteSpace(request.FirstName)
        )
        {
            return Result.Failure<CreateEmployeeResult>(
                "Email, password and first name are required."
            );
        }

        var existing = await _userManager.FindByEmailAsync(request.Email);
        if (existing is not null)
        {
            return Result.Failure<CreateEmployeeResult>("A user with this email already exists.");
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserType = UserType.Employee,
            EmailConfirmed = true,
            IsActive = true,
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            var errorMessage = string.Join("; ", result.Errors.Select(e => e.Description));
            return Result.Failure<CreateEmployeeResult>(errorMessage);
        }

        const string employeeRole = "Employee";

        if (!await _roleManager.RoleExistsAsync(employeeRole))
        {
            await _roleManager.CreateAsync(new ApplicationRole(employeeRole));
        }

        await _userManager.AddToRoleAsync(user, employeeRole);

        var dto = new CreateEmployeeResult(user.Id, user.Email ?? string.Empty, user.FullName);

        return Result.Success(dto);
    }
}
