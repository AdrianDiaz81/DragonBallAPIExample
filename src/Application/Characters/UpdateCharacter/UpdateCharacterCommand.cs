namespace Application.Characters.UpdateCharacter;

public sealed record UpdateCharacterCommand(
    int Id,
    string? Name = null,
    string? LastName = null,
    string? Race = null,
    int? PowerLevel = null,
    string? Description = null,
    string? Affiliation = null,
    string? ImageUrl = null);
