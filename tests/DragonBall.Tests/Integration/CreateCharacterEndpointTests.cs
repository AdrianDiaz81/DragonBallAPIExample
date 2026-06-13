using System.Net;
using System.Net.Http.Json;
using Domain.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DragonBall.Tests.Integration;

public sealed class CreateCharacterEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task POST_characters_with_valid_body_returns_201()
    {
        var body = new { Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_characters_returns_created_character_with_id_and_last_name()
    {
        var body = new { Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);
        var character = await response.Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        character.Should().NotBeNull();
        character!.Id.Should().BeGreaterThan(0);
        character.Name.Should().Be("Bardock");
        character.LastName.Should().Be("Bardock");
    }

    [Fact]
    public async Task POST_characters_returns_location_header()
    {
        var body = new { Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith("/characters/");
    }

    [Fact]
    public async Task POST_characters_with_zero_power_level_returns_201()
    {
        var body = new { Name = "Goku", LastName = "Son", Race = "Saiyan", PowerLevel = 0 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_characters_with_empty_name_returns_422()
    {
        var body = new { Name = "", LastName = "Son", Race = "Saiyan", PowerLevel = 9000 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task POST_characters_with_empty_last_name_returns_422()
    {
        var body = new { Name = "Goku", LastName = "", Race = "Saiyan", PowerLevel = 9000 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task POST_characters_with_negative_power_level_returns_422()
    {
        var body = new { Name = "Goku", LastName = "Son", Race = "Saiyan", PowerLevel = -1 };

        var response = await _client.PostAsJsonAsync("/characters", body, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
