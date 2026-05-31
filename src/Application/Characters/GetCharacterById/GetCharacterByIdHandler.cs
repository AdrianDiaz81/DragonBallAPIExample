using Domain.Characters;

namespace Application.Characters.GetCharacterById;

public sealed class GetCharacterByIdHandler(ICharacterRepository repository)
{
    public Task<Character?> Handle(GetCharacterByIdQuery query, CancellationToken cancellationToken)
        => Task.FromResult(repository.GetById(query.Id));
}
