using Application.Characters.DeleteCharacter;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record DeleteCharacter() : Delete("/characters/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
            .Produces(204)
            .ProducesProblem(404);

    public async Task<IResult> HandleAsync(
        int id,
        [FromServices] DeleteCharacterHandler handler,
        CancellationToken cancellationToken)
    {
        var deleted = await handler.Handle(new DeleteCharacterCommand(id), cancellationToken);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
