namespace Domain.Characters;

public interface ICharacterRepository
{
    IReadOnlyList<Character> GetAll();
    Character? GetById(int id);
    Character Add(Character character);
    Character? Update(int id, string? name, string? lastName, string? race,
        int? powerLevel, string? description, string? affiliation, string? imageUrl);
    bool Delete(int id);
}
