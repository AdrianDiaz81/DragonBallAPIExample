using System.Net;
using System.Net.Http.Json;
using Domain.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DragonBall.Tests.Integration;

public sealed class GetCharactersEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GET_characters_returns_200()
    {
        var response = await _client.GetAsync("/characters", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GET_characters_returns_list_of_characters()
    {
        var response = await _client.GetAsync("/characters", TestContext.Current.CancellationToken);
        var characters = await response.Content.ReadFromJsonAsync<List<Character>>(TestContext.Current.CancellationToken);

        characters.Should().NotBeNullOrEmpty();
        characters.Should().Contain(c => c.Name == "Goku" && c.LastName == "Son");
    }

    [Fact]
    public async Task GET_characters_returns_characters_with_last_name()
    {
        var response = await _client.GetAsync("/characters", TestContext.Current.CancellationToken);
        var characters = await response.Content.ReadFromJsonAsync<List<Character>>(TestContext.Current.CancellationToken);

        characters.Should().AllSatisfy(c => c.LastName.Should().NotBeNullOrEmpty());
    }
}
