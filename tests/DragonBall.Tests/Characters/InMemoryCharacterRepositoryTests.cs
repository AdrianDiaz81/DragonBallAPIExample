using Application.Characters;
using Domain.Characters;
using FluentAssertions;

namespace DragonBall.Tests.Characters;

public sealed class InMemoryCharacterRepositoryTests
{
    private readonly InMemoryCharacterRepository _repository = new();

    [Fact]
    public void GetAll_returns_seeded_characters()
    {
        var characters = _repository.GetAll();
        characters.Should().NotBeEmpty();
        characters.Should().Contain(c => c.Name == "Goku");
    }

    [Fact]
    public void Add_returns_character_with_generated_id()
    {
        var input = new Character { Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };

        var result = _repository.Add(input);

        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Bardock");
    }

    [Fact]
    public void Add_persists_character_in_GetAll()
    {
        var input = new Character { Name = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None" };
        var added = _repository.Add(input);

        _repository.GetAll().Should().Contain(c => c.Id == added.Id);
    }

    [Fact]
    public void Add_increments_id_for_each_character()
    {
        var first = _repository.Add(new Character { Name = "A", Race = "R", PowerLevel = 1, Affiliation = "X" });
        var second = _repository.Add(new Character { Name = "B", Race = "R", PowerLevel = 1, Affiliation = "X" });

        second.Id.Should().BeGreaterThan(first.Id);
    }
}
