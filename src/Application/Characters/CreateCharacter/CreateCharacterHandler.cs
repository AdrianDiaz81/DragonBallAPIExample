using Domain.Characters;
using FluentValidation;

namespace Application.Characters.CreateCharacter;

public sealed class CreateCharacterHandler(
    ICharacterRepository repository,
    IValidator<CreateCharacterCommand> validator)
{
    public async Task<Character> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var character = new Character
        {
            Name = command.Name,
            Race = command.Race,
            PowerLevel = command.PowerLevel,
            Affiliation = command.Affiliation
        };

        return repository.Add(character);
    }
}
