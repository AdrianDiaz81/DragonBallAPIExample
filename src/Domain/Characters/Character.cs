namespace Domain.Characters;

public sealed class Character
{
    public int Id { get; init; }
    public string Name { get; init; } = default!;
    public string Race { get; init; } = default!;
    public int PowerLevel { get; init; }
    public string Affiliation { get; init; } = default!;
    public string ImageUrl { get; init; } = default!;
}
