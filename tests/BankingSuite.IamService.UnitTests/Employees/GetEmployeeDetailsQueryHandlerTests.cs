using BankingSuite.IamService.Application.Employees.Queries.GetEmployeeDetails;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Employees;

public class GetEmployeeDetailsQueryHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public GetEmployeeDetailsQueryHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();
        _roleManager = _provider.GetRequiredService<RoleManager<ApplicationRole>>();
    }

    [Fact]
    public async Task Handle_Should_Return_Details_When_User_Exists()
    {
        await _roleManager.CreateAsync(new ApplicationRole { Name = "Employee" });
        var user = new ApplicationUser
        {
            UserName = "detail@test.com",
            Email = "detail@test.com",
            FirstName = "Detail",
            LastName = "User",
            PhoneNumber = "555"
        };

        await _userManager.CreateAsync(user, "Pass123");
        await _userManager.AddToRoleAsync(user, "Employee");

        var handler = new GetEmployeeDetailsQueryHandler(_userManager);
        var result = await handler.Handle(new GetEmployeeDetailsQuery(user.Id), CancellationToken.None);

        result.Should().NotBeNull();
        result!.Email.Should().Be("detail@test.com");
        result.FullName.Should().Contain("Detail");
        result.Roles.Should().Contain("Employee");
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_Not_Found()
    {
        var handler = new GetEmployeeDetailsQueryHandler(_userManager);

        var result = await handler.Handle(new GetEmployeeDetailsQuery(Guid.NewGuid()), CancellationToken.None);

        result.Should().BeNull();
    }

    public void Dispose() => _provider.Dispose();
}
