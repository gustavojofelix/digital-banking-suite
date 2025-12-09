using BankingSuite.IamService.Application.Employees.Queries.ListEmployees;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.UnitTests.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.UnitTests.Employees;

public class ListEmployeesQueryHandlerTests : IDisposable
{
    private readonly ServiceProvider _provider;
    private readonly UserManager<ApplicationUser> _userManager;

    public ListEmployeesQueryHandlerTests()
    {
        _provider = IdentityTestHelper.BuildProvider();
        _userManager = _provider.GetRequiredService<UserManager<ApplicationUser>>();
    }

    [Fact]
    public async Task Handle_Should_Filter_Inactive_And_Search()
    {
        await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = "active@test.com",
            Email = "active@test.com",
            FirstName = "Active",
            LastName = "User",
            IsActive = true
        }, "Pass123");

        await _userManager.CreateAsync(new ApplicationUser
        {
            UserName = "inactive@test.com",
            Email = "inactive@test.com",
            FirstName = "Inactive",
            LastName = "User",
            IsActive = false
        }, "Pass123");

        var handler = new ListEmployeesQueryHandler(_userManager);
        var query = new ListEmployeesQuery(
            PageNumber: 1,
            PageSize: 10,
            Search: "active",
            IncludeInactive: false);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.Items.Single().Email.Should().Be("active@test.com");
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task Handle_Should_Page_Results()
    {
        for (var i = 0; i < 3; i++)
        {
            await _userManager.CreateAsync(new ApplicationUser
            {
                UserName = $"u{i}@test.com",
                Email = $"u{i}@test.com",
                FirstName = "User",
                LastName = i.ToString(),
                IsActive = true
            }, "Pass123");
        }

        var handler = new ListEmployeesQueryHandler(_userManager);
        var query = new ListEmployeesQuery(PageNumber: 2, PageSize: 2, Search: null, IncludeInactive: true);

        var result = await handler.Handle(query, CancellationToken.None);

        result.Items.Should().HaveCount(1);
        result.PageNumber.Should().Be(2);
        result.PageSize.Should().Be(2);
        result.TotalCount.Should().Be(3);
    }

    public void Dispose() => _provider.Dispose();
}
