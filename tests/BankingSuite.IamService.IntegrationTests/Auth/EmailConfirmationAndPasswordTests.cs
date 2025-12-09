using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Web;
using BankingSuite.IamService.IntegrationTests.Infrastructure;
using FastEndpoints;
using FluentAssertions;

namespace BankingSuite.IamService.IntegrationTests.Auth;

public sealed class EmailConfirmationAndPasswordTests(IamApiFactory factory)
    : IntegrationTestBase(factory)
{
    private const string Password = "Password123!";

    [Fact]
    public async Task ResendConfirmation_ThenConfirmEmail_ShouldMarkEmailAsConfirmed()
    {
        // Arrange
        var email = "employee@alvorbank.test";
        var user = await CreateEmployeeAsync(email, Password, emailConfirmed: false);

        // Act: request resend confirmation
        var resendResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/resend-confirmation",
            new { email }
        );

        resendResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        Factory.SentEmails.Should().NotBeEmpty();

        var confirmationEmail = Factory.SentEmails.Last();
        confirmationEmail.To.Should().Be(email);

        // Extract link and token from email body (simple parse for demo)
        var link = ExtractFirstHref(confirmationEmail.HtmlBody);
        link.Should().NotBeNullOrWhiteSpace();

        var uri = new Uri(link);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var userId = Guid.Parse(query["userId"]!);
        var token = query["token"]!;

        // Call confirm-email endpoint
        var confirmResponse = await Client.GetAsync(
            $"/api/iam/auth/confirm-email?userId={userId}&token={HttpUtility.UrlEncode(token)}"
        );

        confirmResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try to log in (should work if email is now confirmed)
        var loginResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/login",
            new { email, password = Password }
        );

        loginResponse.EnsureSuccessStatusCode();
    }

    [Fact]
    public async Task ForgotPassword_ThenResetPassword_ShouldAllowLoginWithNewPassword()
    {
        // Arrange
        var email = "reset@alvorbank.test";
        var user = await CreateEmployeeAsync(email, Password, emailConfirmed: true);

        // Act: forgot password
        var forgotResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/forgot-password",
            new { email }
        );

        forgotResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        Factory.SentEmails.Should().NotBeEmpty();
        var resetEmail = Factory.SentEmails.Last();
        resetEmail.To.Should().Be(email);

        var link = ExtractFirstHref(resetEmail.HtmlBody);
        link.Should().NotBeNullOrWhiteSpace();

        var uri = new Uri(link);
        var query = HttpUtility.ParseQueryString(uri.Query);
        var token = query["token"]!;
        var encodedEmail = query["email"]!;
        var decodedEmail = HttpUtility.UrlDecode(encodedEmail);

        // Reset password
        const string newPassword = "NewPassword123!";

        var resetResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/reset-password",
            new
            {
                email = decodedEmail,
                token,
                newPassword,
            }
        );

        resetResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Try to log in with the new password
        var loginResponse = await Client.PostAsJsonAsync(
            "/api/iam/auth/login",
            new { email = decodedEmail, password = newPassword }
        );

        loginResponse.EnsureSuccessStatusCode();
    }

    private static string ExtractFirstHref(string html)
    {
        const string marker = "href=\"";
        var start = html.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
            return string.Empty;
        start += marker.Length;
        var end = html.IndexOf("\"", start, StringComparison.OrdinalIgnoreCase);
        if (end < 0)
            return string.Empty;
        return html[start..end];
    }
}
