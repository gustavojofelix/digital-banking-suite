using BankingSuite.IamService.Application.Auth.Commands.CreateEmployee;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Auth;

public class CreateEmployeeCommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public CreateEmployeeCommandHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _provider.GetRequiredService<RoleManager<ApplicationRole>>();
    }

    [Fact]
    public async Task Handle_Should_Create_User_And_Assign_Employee_Role()
    {
        var handler = new CreateEmployeeCommandHandler(_userManager, _roleManager);
        var command = new CreateEmployeeCommand(
            Email: "new.user@test.com",
            Password: "Secret1",
            FirstName: "New",
            LastName: "User");

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        var created = await _userManager.FindByEmailAsync(command.Email);
        created.Should().NotBeNull();
        created!.FirstName.Should().Be("New");
        (await _userManager.IsInRoleAsync(created, "Employee")).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_When_Email_Exists_Should_Return_Failure()
    {
        var existing = new ApplicationUser { UserName = "dup@test.com", Email = "dup@test.com" };
        await _userManager.CreateAsync(existing, "Secret1");

        var handler = new CreateEmployeeCommandHandler(_userManager, _roleManager);
        var duplicate = new CreateEmployeeCommand(
            Email: "dup@test.com",
            Password: "Secret1",
            FirstName: "Dup",
            LastName: "User");

        var result = await handler.Handle(duplicate, CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Should().Contain("already exists");
    }

    public void Dispose() => _provider.Dispose();
}
