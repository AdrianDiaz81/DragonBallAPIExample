using Application.Characters.DeleteCharacter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record DeleteCharacter() : Delete("/characters/{id}")
{
    public async Task<IResult> HandleAsync(
        int id,
        [FromServices] DeleteCharacterHandler handler,
        CancellationToken cancellationToken)
    {
        var deleted = await handler.Handle(new DeleteCharacterCommand(id), cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
