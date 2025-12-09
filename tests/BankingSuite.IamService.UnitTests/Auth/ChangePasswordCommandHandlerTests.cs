using BankingSuite.IamService.Application.Auth.Commands.ChangePassword;
using BankingSuite.IamService.Application.Common.Interfaces;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Auth;

public class ChangePasswordCommandHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationUser _user;

    public ChangePasswordCommandHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();

        _user = new ApplicationUser { UserName = "user@test.com", Email = "user@test.com" };
        _userManager.CreateAsync(_user, "OldPass1").GetAwaiter().GetResult();
    }

    [Fact]
    public async Task Handle_Should_Update_Password_When_Current_Is_Correct()
    {
        var currentUser = new FakeCurrentUser(_user.Id);
        var handler = new ChangePasswordCommandHandler(_userManager, currentUser);
        var command = new ChangePasswordCommand(CurrentPassword: "OldPass1", NewPassword: "NewPass1");

        await handler.Handle(command, CancellationToken.None);

        (await _userManager.CheckPasswordAsync(_user, "OldPass1")).Should().BeFalse();
        (await _userManager.CheckPasswordAsync(_user, "NewPass1")).Should().BeTrue();
    }

    [Fact]
    public async Task Handle_With_Invalid_CurrentPassword_Should_Throw()
    {
        var currentUser = new FakeCurrentUser(_user.Id);
        var handler = new ChangePasswordCommandHandler(_userManager, currentUser);
        var command = new ChangePasswordCommand(CurrentPassword: "Wrong", NewPassword: "Another1");

        var act = () => handler.Handle(command, CancellationToken.None);

        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("*Failed to change password*");
    }

    public void Dispose() => _provider.Dispose();

    private sealed class FakeCurrentUser(Guid id) : ICurrentUser
    {
        public Guid UserId => id;
        public string? Email => null;
        public bool IsAuthenticated => true;
    }
}
