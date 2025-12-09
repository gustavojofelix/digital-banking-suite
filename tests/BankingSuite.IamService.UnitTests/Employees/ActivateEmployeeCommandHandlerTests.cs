using BankingSuite.IamService.Application.Employees.Commands.ActivateEmployee;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Employees;

public class ActivateEmployeeCommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;

    public ActivateEmployeeCommandHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task Handle_Should_Enable_User()
    {
        var user = new ApplicationUser
        {
            UserName = "locked@test.com",
            Email = "locked@test.com",
            IsActive = false,
            LockoutEnd = DateTimeOffset.UtcNow.AddYears(1)
        };

        await _userManager.CreateAsync(user, "Pass123");

        var handler = new ActivateEmployeeCommandHandler(_userManager);
        await handler.Handle(new ActivateEmployeeCommand(user.Id), CancellationToken.None);

        var updated = await _userManager.FindByIdAsync(user.Id.ToString());
        updated!.IsActive.Should().BeTrue();
        updated.LockoutEnd.Should().BeNull();
    }

    [Fact]
    public async Task Handle_When_User_Not_Found_Should_Throw()
    {
        var handler = new ActivateEmployeeCommandHandler(_userManager);

        await FluentActions.Invoking(() => handler.Handle(new ActivateEmployeeCommand(Guid.NewGuid()), CancellationToken.None))
            .Should()
            .ThrowAsync<KeyNotFoundException>();
    }

    public void Dispose() => _provider.Dispose();
}
