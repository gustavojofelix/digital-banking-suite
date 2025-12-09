using System.Net;
using System.Net.Http.Json;
using BankingSuite.IamService.IntegrationTests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BankingSuite.IamService.IntegrationTests.Health;

// Define a collection for the IamApiFactory fixture
[CollectionDefinition("IamApiFactory collection")]
public class IamApiFactoryCollection : ICollectionFixture<IamApiFactory>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[Collection("IamApiFactory collection")]
public class HealthEndpointTests
{
    private readonly HttpClient _client;

    public HealthEndpointTests(IamApiFactory factory)
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
