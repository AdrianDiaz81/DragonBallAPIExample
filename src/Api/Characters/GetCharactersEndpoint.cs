using Application.Characters.GetCharacters;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

public record GetCharacters() : Get("/characters")
{
    public async Task<IResult> HandleAsync([FromServices] GetCharactersHandler handler, CancellationToken cancellationToken)
    {
        var characters = await handler.Handle(new GetCharactersQuery(), cancellationToken);
        return Results.Ok(characters);
    }
}
