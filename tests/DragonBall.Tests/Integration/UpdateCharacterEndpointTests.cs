using System.Net;
using System.Net.Http.Json;
using Domain.Characters;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace DragonBall.Tests.Integration;

public sealed class UpdateCharacterEndpointTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    private async Task<Character> CreateTestCharacterAsync() =>
        (await (await _client.PostAsJsonAsync(
            "/characters",
            new { Name = "Test", LastName = "Update", PowerLevel = 500 },
            TestContext.Current.CancellationToken))
        .Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken))!;

    [Fact]
    public async Task PUT_existing_character_with_power_level_returns_200()
    {
        var created = await CreateTestCharacterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/characters/{created.Id}", new { PowerLevel = 150 }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task PUT_only_updates_sent_fields_and_leaves_others_unchanged()
    {
        var created = await CreateTestCharacterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/characters/{created.Id}", new { PowerLevel = 150 }, TestContext.Current.CancellationToken);
        var updated = await response.Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        updated!.PowerLevel.Should().Be(150);
        updated.Name.Should().Be(created.Name);
        updated.LastName.Should().Be(created.LastName);
    }

    [Fact]
    public async Task PUT_non_existent_id_returns_404()
    {
        var response = await _client.PutAsJsonAsync(
            "/characters/9999", new { PowerLevel = 1 }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task PUT_with_empty_name_returns_422()
    {
        var created = await CreateTestCharacterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/characters/{created.Id}", new { Name = "" }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task PUT_with_negative_power_level_returns_422()
    {
        var created = await CreateTestCharacterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/characters/{created.Id}", new { PowerLevel = -1 }, TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
    }

    [Fact]
    public async Task PUT_with_empty_body_returns_200_without_changes()
    {
        var created = await CreateTestCharacterAsync();

        var response = await _client.PutAsJsonAsync(
            $"/characters/{created.Id}", new { }, TestContext.Current.CancellationToken);
        var result = await response.Content.ReadFromJsonAsync<Character>(TestContext.Current.CancellationToken);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        result!.Name.Should().Be(created.Name);
        result.PowerLevel.Should().Be(created.PowerLevel);
    }
}
