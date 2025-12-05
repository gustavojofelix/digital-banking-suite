using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BankingSuite.IamService.IntegrationTests.Health;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_Should_Return_Healthy_Status()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<HealthResponse>();

        payload.Should().NotBeNull();
        payload!.Service.Should().Be("IamService");
        payload.Status.Should().Be("Healthy");
    }

    private sealed class HealthResponse
    {
        public string? Service { get; set; }
        public string? Status { get; set; }
        public DateTime TimestampUtc { get; set; }
    }
}
