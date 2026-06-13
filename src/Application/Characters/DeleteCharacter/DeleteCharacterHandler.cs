using Domain.Characters;
using Microsoft.Extensions.Logging;

namespace Application.Characters.DeleteCharacter;

public sealed partial class DeleteCharacterHandler(
    ICharacterRepository repository,
    ILogger<DeleteCharacterHandler> logger)
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Character deleted: {Id}")]
    private static partial void LogCharacterDeleted(ILogger logger, int id);

    public Task<bool> Handle(DeleteCharacterCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var deleted = repository.Delete(command.Id);
        if (deleted) LogCharacterDeleted(logger, command.Id);
        return Task.FromResult(deleted);
    }
}
