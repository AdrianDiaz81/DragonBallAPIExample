using Application.Characters.GetCharacters;
using Domain.Characters;
using FluentAssertions;
using NSubstitute;

namespace DragonBall.Tests.Characters;

public sealed class GetCharactersHandlerTests
{
    private readonly ICharacterRepository _repository = Substitute.For<ICharacterRepository>();
    private readonly GetCharactersHandler _handler;

    public GetCharactersHandlerTests()
    {
        _handler = new GetCharactersHandler(_repository);
    }

    [Fact]
    public async Task Returns_all_characters_from_repository()
    {
        var expected = new List<Character>
        {
            new() { Id = 1, Name = "Goku", Race = "Saiyan", PowerLevel = 9000, Affiliation = "Z Fighters", ImageUrl = "https://example.com/goku.webp" }
        };
        _repository.GetAll().Returns(expected);

        var result = await _handler.Handle(new GetCharactersQuery(), CancellationToken.None);

        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task Returns_empty_list_when_repository_is_empty()
    {
        _repository.GetAll().Returns([]);

        var result = await _handler.Handle(new GetCharactersQuery(), CancellationToken.None);

        result.Should().BeEmpty();
    }
}
