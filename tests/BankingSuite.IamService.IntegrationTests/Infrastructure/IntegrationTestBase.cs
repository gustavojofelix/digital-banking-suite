using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using BankingSuite.IamService.Domain.Users;
using BankingSuite.IamService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankingSuite.IamService.IntegrationTests.Infrastructure;

public abstract class IntegrationTestBase : IClassFixture<IamApiFactory>, IAsyncLifetime
{
    protected readonly IamApiFactory Factory;
    protected readonly HttpClient Client;

    protected IntegrationTestBase(IamApiFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<IamDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

        Factory.SentEmails.Clear();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    protected async Task<ApplicationUser> CreateEmployeeAsync(
        string email,
        string password,
        bool isActive = true,
        bool emailConfirmed = true,
        bool twoFactorEnabled = false,
        string? fullName = null
    )
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            //FullName = fullName ?? email,
            IsActive = isActive,
            EmailConfirmed = emailConfirmed,
            TwoFactorEnabled = twoFactorEnabled,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Failed to create test user: {errors}");
        }

        var persisted = await userManager.FindByEmailAsync(email);
        if (persisted is null)
        {
            throw new InvalidOperationException("User not found immediately after creation.");
        }

        return user;
    }

    protected async Task AddRoleAsync(Guid userId, string roleName)
    {
        using var scope = Factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }

        // 👇 Load a fresh tracked instance in this context
        var user = await userManager.FindByIdAsync(userId.ToString())
                   ?? throw new InvalidOperationException("Test user not found.");

        await userManager.AddToRoleAsync(user, roleName);
    }

    protected async Task<string> AuthenticateAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/iam/auth/login", new { email, password });

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            using var scope = Factory.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByEmailAsync(email);
            var passwordValid = user is not null && await userManager.CheckPasswordAsync(user, password);

            throw new InvalidOperationException(
                $"Login failed ({response.StatusCode}): {body} | userExists={user is not null}, passwordValid={passwordValid}"
            );
        }

        var json = await response.Content.ReadFromJsonAsync<LoginResponseForTests>();

        if (json is null || json.AccessToken is null)
            throw new InvalidOperationException("Login did not return an access token.");

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", json.AccessToken);

        return json.AccessToken;
    }

    private sealed class LoginResponseForTests
    {
        public bool RequiresTwoFactor { get; init; }
        public Guid? UserId { get; init; }
        public string? AccessToken { get; init; }
        public DateTimeOffset? ExpiresAt { get; init; }
    }
}
