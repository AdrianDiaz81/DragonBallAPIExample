using System.Net;
using System.Net.Http.Json;
using Domain.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DragonBall.Tests.Integration;

public sealed class DeleteCharacterEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task DELETE_existing_character_returns_204()
    {
        // Crear un personaje para no afectar al dataset compartido
        var created = await (await _client.PostAsJsonAsync(
            "/characters",
            new { Name = "Test", LastName = "Delete", PowerLevel = 1 },
            TestContext.Current.CancellationToken))
            .Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        var response = await _client.DeleteAsync($"/characters/{created!.Id}", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task DELETE_non_existent_id_returns_404()
    {
        var response = await _client.DeleteAsync("/characters/9999", TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DELETE_then_GET_returns_404()
    {
        var created = await (await _client.PostAsJsonAsync(
            "/characters",
            new { Name = "ToDelete", LastName = "Now", PowerLevel = 1 },
            TestContext.Current.CancellationToken))
            .Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        await _client.DeleteAsync($"/characters/{created!.Id}", TestContext.Current.CancellationToken);

        var getResponse = await _client.GetAsync($"/characters/{created.Id}", TestContext.Current.CancellationToken);
        getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GET_characters_after_delete_returns_200_with_remaining()
    {
        var created = await (await _client.PostAsJsonAsync(
            "/characters",
            new { Name = "Temp", LastName = "Char", PowerLevel = 1 },
            TestContext.Current.CancellationToken))
            .Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        await _client.DeleteAsync($"/characters/{created!.Id}", TestContext.Current.CancellationToken);

        var listResponse = await _client.GetAsync("/characters", TestContext.Current.CancellationToken);
        var characters = await listResponse.Content.ReadFromJsonAsync<List<Character>>(TestContext.Current.CancellationToken);

        listResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        characters.Should().NotContain(c => c.Id == created.Id);
    }
}
