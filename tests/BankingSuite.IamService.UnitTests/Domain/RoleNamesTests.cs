using BankingSuite.IamService.Domain.Users;
using FluentAssertions;

namespace BankingSuite.IamService.UnitTests.Domain;

public class RoleNamesTests
{
    [Fact]
    public void All_Should_List_All_Known_Roles()
    {
        RoleNames.All.Should().Contain(RoleNames.SystemAdmin);
        RoleNames.All.Should().Contain(RoleNames.Customer);
        RoleNames.All.Distinct().Should().HaveCount(RoleNames.All.Count);
    }
}
