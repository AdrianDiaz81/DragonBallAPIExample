using Domain.Characters;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Characters.UpdateCharacter;

public sealed partial class UpdateCharacterHandler(
    ICharacterRepository repository,
    IValidator<UpdateCharacterCommand> validator,
    ILogger<UpdateCharacterHandler> logger)
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Character updated: {Id}")]
    private static partial void LogCharacterUpdated(ILogger logger, int id);

    public async Task<Character?> Handle(UpdateCharacterCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var patch = new CharacterPatch(
            command.Name,
            command.LastName,
            command.Race,
            command.PowerLevel,
            command.Description,
            command.Affiliation,
            command.ImageUrl);

        var character = repository.Update(command.Id, patch);

        if (character is not null)
            LogCharacterUpdated(logger, character.Id);

        return character;
    }
}
