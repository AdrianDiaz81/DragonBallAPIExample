namespace Domain.Characters;

public sealed record CharacterPatch(
    string? Name = null,
    string? LastName = null,
    string? Race = null,
    int? PowerLevel = null,
    string? Description = null,
    string? Affiliation = null,
    string? ImageUrl = null);
