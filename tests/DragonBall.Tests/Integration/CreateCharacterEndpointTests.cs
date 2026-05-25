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
        var body = new { Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };

        var response = await _client.PostAsJsonAsync("/characters", body);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task POST_characters_returns_created_character_with_id()
    {
        var body = new { Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };

        var response = await _client.PostAsJsonAsync("/characters", body);
        var character = await response.Content.ReadFromJsonAsync<Character>();

        character.Should().NotBeNull();
        character!.Id.Should().BeGreaterThan(0);
        character.Name.Should().Be("Bardock");
    }

    [Fact]
    public async Task POST_characters_returns_location_header()
    {
        var body = new { Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };

        var response = await _client.PostAsJsonAsync("/characters", body);

        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().StartWith("/characters/");
    }

    [Fact]
    public async Task POST_characters_with_empty_name_returns_422()
    {
        var body = new { Name = "", Race = "Saiyan", PowerLevel = 9000, Affiliation = "Z Fighters" };

        var response = await _client.PostAsJsonAsync("/characters", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task POST_characters_with_zero_power_level_returns_422()
    {
        var body = new { Name = "Goku", Race = "Saiyan", PowerLevel = 0, Affiliation = "Z Fighters" };

        var response = await _client.PostAsJsonAsync("/characters", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task POST_characters_with_missing_fields_returns_422()
    {
        var body = new { Name = "", Race = "", PowerLevel = -1, Affiliation = "" };

        var response = await _client.PostAsJsonAsync("/characters", body);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }
}
