using Domain.Characters;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace Application.Characters.CreateCharacter;

public sealed partial class CreateCharacterHandler(
    ICharacterRepository repository,
    IValidator<CreateCharacterCommand> validator,
    ILogger<CreateCharacterHandler> logger)
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Character created: {Id} — {Name} {LastName}")]
    private static partial void LogCharacterCreated(ILogger logger, int id, string name, string lastName);

    public async Task<Character> Handle(CreateCharacterCommand command, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(command, cancellationToken);

        var character = repository.Add(new Character
        {
            Name = command.Name,
            LastName = command.LastName,
            Race = command.Race,
            PowerLevel = command.PowerLevel,
            Description = command.Description,
            Affiliation = command.Affiliation,
            ImageUrl = command.ImageUrl
        });

        LogCharacterCreated(logger, character.Id, character.Name, character.LastName);
        return character;
    }
}
