namespace Domain.Characters;

public interface ICharacterRepository
{
    IReadOnlyList<Character> GetAll();
    Character Add(Character character);
}
