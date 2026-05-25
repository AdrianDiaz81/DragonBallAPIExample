using Domain.Characters;

namespace Application.Characters;

public sealed class InMemoryCharacterRepository : ICharacterRepository
{
    private readonly List<Character> _characters =
    [
        new() { Id = 1,  Name = "Goku",      Race = "Saiyan",       PowerLevel = 9000,    Affiliation = "Z Fighters" },
        new() { Id = 2,  Name = "Vegeta",     Race = "Saiyan",       PowerLevel = 8500,    Affiliation = "Z Fighters" },
        new() { Id = 3,  Name = "Gohan",      Race = "Half-Saiyan",  PowerLevel = 7000,    Affiliation = "Z Fighters" },
        new() { Id = 4,  Name = "Piccolo",    Race = "Namekian",     PowerLevel = 5000,    Affiliation = "Z Fighters" },
        new() { Id = 5,  Name = "Frieza",     Race = "Frieza Race",  PowerLevel = 120000,  Affiliation = "Frieza Force" },
        new() { Id = 6,  Name = "Cell",       Race = "Bio-Android",  PowerLevel = 900000,  Affiliation = "Red Ribbon Army" },
        new() { Id = 7,  Name = "Majin Buu",  Race = "Majin",        PowerLevel = 1000000, Affiliation = "None" },
        new() { Id = 8,  Name = "Trunks",     Race = "Half-Saiyan",  PowerLevel = 6500,    Affiliation = "Z Fighters" },
        new() { Id = 9,  Name = "Krillin",    Race = "Human",        PowerLevel = 1770,    Affiliation = "Z Fighters" },
        new() { Id = 10, Name = "Broly",      Race = "Saiyan",       PowerLevel = 1400000, Affiliation = "None" },
    ];

    public IReadOnlyList<Character> GetAll() => _characters.AsReadOnly();

    public Character Add(Character character)
    {
        var newId = _characters.Count > 0 ? _characters.Max(c => c.Id) + 1 : 1;
        var newCharacter = new Character
        {
            Id = newId,
            Name = character.Name,
            Race = character.Race,
            PowerLevel = character.PowerLevel,
            Affiliation = character.Affiliation
        };
        _characters.Add(newCharacter);
        return newCharacter;
    }
}
