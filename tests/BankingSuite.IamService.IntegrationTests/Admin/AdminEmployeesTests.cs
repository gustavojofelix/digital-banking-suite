using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using BankingSuite.IamService.IntegrationTests.Infrastructure;
using FluentAssertions;

namespace BankingSuite.IamService.IntegrationTests.Admin;

public sealed class AdminEmployeesTests(IamApiFactory factory) : IntegrationTestBase(factory)
{
    private const string AdminRole = "IamAdmin";
    private const string AdminEmail = "admin@alvorbank.test";
    private const string AdminPassword = "Admin123!";

    [Fact]
    public async Task ListEmployees_ReturnsPagedResult_ForAdmin()
    {
        // Arrange
        var admin = await CreateEmployeeAsync(AdminEmail, AdminPassword, emailConfirmed: true);
        await AddRoleAsync(admin.Id, AdminRole);

        var emp1 = await CreateEmployeeAsync(
            "e1@alvorbank.test",
            "Password123!",
            emailConfirmed: true
        );
        var emp2 = await CreateEmployeeAsync(
            "e2@alvorbank.test",
            "Password123!",
            emailConfirmed: true,
            isActive: false
        );

        await AuthenticateAsync(AdminEmail, AdminPassword);

        // Act
        var response = await Client.GetAsync(
            "/api/iam/admin/employees?pageNumber=1&pageSize=10&includeInactive=true"
        );

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<PagedEmployeesResponse>();

        result.Should().NotBeNull();
        result!.Items.Should().HaveCount(3); // admin + 2 employees
        result.Items.Any(e => e.Email == emp1.Email).Should().BeTrue();
        result.Items.Any(e => e.Email == emp2.Email).Should().BeTrue();
    }

    [Fact]
    public async Task DeactivateEmployee_ThenLogin_ShouldFail()
    {
        // Arrange
        var admin = await CreateEmployeeAsync(AdminEmail, AdminPassword, emailConfirmed: true);
        await AddRoleAsync(admin.Id, AdminRole);

        var employee = await CreateEmployeeAsync(
            "user@alvorbank.test",
            "Password123!",
            emailConfirmed: true
        );

        await AuthenticateAsync(AdminEmail, AdminPassword);

        // Act: deactivate the employee
        var deactivateResponse = await Client.PostAsync(
            $"/api/iam/admin/employee/{employee.Id}/deactivate",
            content: null
        );
        deactivateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try to log in as that employee
        var loginResponse = await Client.PostAsJsonAsync(
            "/auth/login",
            new { email = employee.Email, password = "Password123!" }
        );

        // Assert: login should fail (we expect a 4xx)
        loginResponse
            .StatusCode.Should()
            .BeOneOf(HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
    }

    private sealed class PagedEmployeesResponse
    {
        public List<EmployeeSummary> Items { get; init; } = [];
        public int TotalCount { get; init; }
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
    }

    private sealed class EmployeeSummary
    {
        public Guid Id { get; init; }
        public string Email { get; init; } = string.Empty;
        public bool IsActive { get; init; }
    }
}
