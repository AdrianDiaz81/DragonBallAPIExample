namespace Domain.Characters;

public sealed class Character
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string LastName { get; init; }
    public string? Race { get; init; }
    public int PowerLevel { get; init; }
    public string? Description { get; init; }
    public string? Affiliation { get; init; }
    public string? ImageUrl { get; init; }
}
