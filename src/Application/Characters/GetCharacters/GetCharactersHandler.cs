using Domain.Characters;

namespace Application.Characters.GetCharacters;

public sealed class GetCharactersHandler(ICharacterRepository repository)
{
    public Task<IReadOnlyList<Character>> Handle(GetCharactersQuery query, CancellationToken cancellationToken)
        => Task.FromResult(repository.GetAll());
}
