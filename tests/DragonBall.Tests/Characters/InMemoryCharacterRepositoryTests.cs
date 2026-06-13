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
    public void GetAll_seeded_characters_have_image_url()
    {
        var characters = _repository.GetAll();
        characters.Should().AllSatisfy(c => c.ImageUrl.Should().NotBeNullOrEmpty());
    }

    [Fact]
    public void Add_returns_character_with_generated_id()
    {
        var input = new Character { Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None", ImageUrl = "https://example.com/bardock.webp" };

        var result = _repository.Add(input);

        result.Id.Should().BeGreaterThan(0);
        result.Name.Should().Be("Bardock");
    }

    [Fact]
    public void Add_persists_character_in_GetAll()
    {
        var input = new Character { Name = "Bardock", LastName = "Bardock", Race = "Saiyan", PowerLevel = 10000, Affiliation = "None", ImageUrl = "https://example.com/bardock.webp" };
        var added = _repository.Add(input);

        _repository.GetAll().Should().Contain(c => c.Id == added.Id);
    }

    [Fact]
    public void Add_increments_id_for_each_character()
    {
        var first = _repository.Add(new Character { Name = "A", LastName = "A", Race = "R", PowerLevel = 1 });
        var second = _repository.Add(new Character { Name = "B", LastName = "B", Race = "R", PowerLevel = 1 });

        second.Id.Should().BeGreaterThan(first.Id);
    }

    [Fact]
    public void Update_only_power_level_leaves_other_fields_unchanged()
    {
        var original = _repository.GetById(1)!;

        var updated = _repository.Update(1, new CharacterPatch(PowerLevel: 99));

        updated.Should().NotBeNull();
        updated!.PowerLevel.Should().Be(99);
        updated.Name.Should().Be(original.Name);
        updated.LastName.Should().Be(original.LastName);
    }

    [Fact]
    public void Update_non_existent_id_returns_null()
    {
        var result = _repository.Update(9999, new CharacterPatch(Name: "X"));
        result.Should().BeNull();
    }

    [Fact]
    public void Delete_existing_character_returns_true_and_removes_it()
    {
        var deleted = _repository.Delete(1);

        deleted.Should().BeTrue();
        _repository.GetById(1).Should().BeNull();
    }

    [Fact]
    public void Delete_non_existent_id_returns_false()
    {
        var deleted = _repository.Delete(9999);
        deleted.Should().BeFalse();
    }

    [Fact]
    public void Delete_all_characters_results_in_empty_collection()
    {
        var all = _repository.GetAll().ToList();
        foreach (var character in all)
            _repository.Delete(character.Id);

        _repository.GetAll().Should().BeEmpty();
    }
}
