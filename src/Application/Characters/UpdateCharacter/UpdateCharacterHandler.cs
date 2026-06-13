using Domain.Characters;
using FluentValidation;

namespace Application.Characters.UpdateCharacter;

public sealed class UpdateCharacterHandler(
    ICharacterRepository repository,
    IValidator<UpdateCharacterCommand> validator)
{
    public async Task<Character?> Handle(UpdateCharacterCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        return repository.Update(
            command.Id,
            command.Name,
            command.LastName,
            command.Race,
            command.PowerLevel,
            command.Description,
            command.Affiliation,
            command.ImageUrl);
    }
}
