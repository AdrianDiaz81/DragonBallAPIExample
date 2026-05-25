namespace Application.Characters.CreateCharacter;

public sealed record CreateCharacterCommand(
    string Name,
    string Race,
    int PowerLevel,
    string Affiliation);
