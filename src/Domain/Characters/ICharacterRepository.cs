namespace Domain.Characters;

public interface ICharacterRepository
{
    IReadOnlyList<Character> GetAll();
    Character? GetById(int id);
    Character Add(Character character);
    Character? Update(int id, CharacterPatch patch);
    bool Delete(int id);
}
