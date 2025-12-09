using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using BankingSuite.IamService.IntegrationTests.Infrastructure;
using FastEndpoints;
using FluentAssertions;

namespace BankingSuite.IamService.IntegrationTests.Auth;

public sealed class TwoFactorTests(IamApiFactory factory) : IntegrationTestBase(factory)
{
    private const string Password = "Password123!";

    [Fact]
    public async Task Login_WithTwoFactorEnabled_ShouldRequire2FA_ThenVerify()
    {
        // Arrange: create a confirmed, active user with 2FA enabled
        var email = "2fa@alvorbank.test";
        var user = await CreateEmployeeAsync(
            email,
            Password,
            emailConfirmed: true,
            twoFactorEnabled: true
        );

        // Act: login with password
        var loginResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/login",
            new { email, password = Password }
        );

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultResponse>();
        loginResult.Should().NotBeNull();
        loginResult!.RequiresTwoFactor.Should().BeTrue();
        loginResult.UserId.Should().NotBeNull();
        loginResult.AccessToken.Should().BeNull();

        // The login should have sent a 2FA email
        Factory.SentEmails.Should().NotBeEmpty();
        var twoFaEmail = Factory.SentEmails.Last();
        twoFaEmail.To.Should().Be(email);

        // Extract the code from the email body (the code is usually plain text, not in a link)
        var code = ExtractCodeFromBody(twoFaEmail.HtmlBody);
        code.Should().NotBeNullOrWhiteSpace();

        // Call 2FA verify endpoint
        var verifyResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/2fa/verify",
            new { userId = loginResult.UserId, code }
        );

        verifyResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var verifyResult = await verifyResponse.Content.ReadFromJsonAsync<LoginResultResponse>();
        verifyResult.Should().NotBeNull();
        verifyResult!.RequiresTwoFactor.Should().BeFalse();
        verifyResult.AccessToken.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public async Task EnableAndDisableTwoFactor_ShouldUpdateUserFlag()
    {
        // Arrange
        var email = "2fasettings@alvorbank.test";
        var user = await CreateEmployeeAsync(email, Password, emailConfirmed: true);

        // Login to get token
        var loginResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/login",
            new { email, password = Password }
        );

        loginResponse.EnsureSuccessStatusCode();

        var loginResult = await loginResponse.Content.ReadFromJsonAsync<LoginResultResponse>();
        var token = loginResult!.AccessToken!;

        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        // Act: enable 2FA
        var enableResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/2fa/enable",
            new { currentPassword = Password }
        );

        enableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try login again, now it should require 2FA
        Client.DefaultRequestHeaders.Authorization = null;

        var login2Response = await Client.PostAsJsonAsync(
            "/api/iam/auth/login",
            new { email, password = Password }
        );

        login2Response.StatusCode.Should().Be(HttpStatusCode.OK);

        var login2Result = await login2Response.Content.ReadFromJsonAsync<LoginResultResponse>();
        login2Result!.RequiresTwoFactor.Should().BeTrue();

        // Act: disable 2FA
        Client.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        var disableResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/2fa/disable",
            new { currentPassword = Password }
        );

        disableResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    private sealed class LoginResultResponse
    {
        public bool RequiresTwoFactor { get; init; }
        public Guid? UserId { get; init; }
        public string? AccessToken { get; init; }
        public DateTimeOffset? ExpiresAt { get; init; }
    }

    private static string ExtractCodeFromBody(string htmlBody)
    {
        // For demo purposes we assume the code is in a <strong> tag.
        const string marker = "<strong>";
        var start = htmlBody.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return string.Empty;
        start += marker.Length;
        var end = htmlBody.IndexOf("</strong>", start, StringComparison.OrdinalIgnoreCase);
        if (end < 0)
            return string.Empty;
        return htmlBody[start..end].Trim();
    }
}
