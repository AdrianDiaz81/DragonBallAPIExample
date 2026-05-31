namespace Domain.Characters;

public interface ICharacterRepository
{
    IReadOnlyList<Character> GetAll();
    Character? GetById(int id);
    Character Add(Character character);
}
