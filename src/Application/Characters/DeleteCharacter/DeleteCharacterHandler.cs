using Domain.Characters;

namespace Application.Characters.DeleteCharacter;

public sealed class DeleteCharacterHandler(ICharacterRepository repository)
{
    public Task<bool> Handle(DeleteCharacterCommand command, CancellationToken cancellationToken)
        => Task.FromResult(repository.Delete(command.Id));
}
