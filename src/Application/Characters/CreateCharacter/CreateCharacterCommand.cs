namespace Application.Characters.CreateCharacter;

public sealed record CreateCharacterCommand(
    string Name,
    string LastName,
    string? Race,
    int PowerLevel,
    string? Description,
    string? Affiliation,
    string? ImageUrl);
