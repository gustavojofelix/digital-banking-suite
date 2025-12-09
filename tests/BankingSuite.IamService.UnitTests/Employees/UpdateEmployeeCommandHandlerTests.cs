using BankingSuite.IamService.Application.Employees.Commands.UpdateEmployee;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Employees;

public class UpdateEmployeeCommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public UpdateEmployeeCommandHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _provider.GetRequiredService<RoleManager<ApplicationRole>>();
    }

    [Fact]
    public async Task Handle_Should_Update_Phone_And_Roles()
    {
        await _roleManager.CreateAsync(new ApplicationRole { Name = "Employee" });
        await _roleManager.CreateAsync(new ApplicationRole { Name = "Manager" });

        var user = new ApplicationUser { UserName = "emp@test.com", Email = "emp@test.com" };
        await _userManager.CreateAsync(user, "Pass123");
        await _userManager.AddToRoleAsync(user, "Employee");

        var handler = new UpdateEmployeeCommandHandler(_userManager);
        var cmd = new UpdateEmployeeCommand(
            EmployeeId: user.Id,
            FullName: null,
            PhoneNumber: "12345",
            Roles: new[] { "Manager" });

        await handler.Handle(cmd, CancellationToken.None);

        var updated = await _userManager.FindByIdAsync(user.Id.ToString());
        updated!.PhoneNumber.Should().Be("12345");
        (await _userManager.IsInRoleAsync(updated, "Manager")).Should().BeTrue();
        (await _userManager.IsInRoleAsync(updated, "Employee")).Should().BeFalse();
    }

    [Fact]
    public async Task Handle_When_User_Not_Found_Should_Throw()
    {
        var handler = new UpdateEmployeeCommandHandler(_userManager);
        var cmd = new UpdateEmployeeCommand(Guid.NewGuid(), null, null, Array.Empty<string>());

        await FluentActions.Invoking(() => handler.Handle(cmd, CancellationToken.None))
            .Should()
            .ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose() => _provider.Dispose();
}
