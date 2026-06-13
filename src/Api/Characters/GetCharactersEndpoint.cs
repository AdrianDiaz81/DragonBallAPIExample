using Application.Characters.GetCharacters;
using Domain.Characters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record GetCharacters() : Get("/characters")
{
    public async Task<IResult> HandleAsync([FromServices] GetCharactersHandler handler, CancellationToken cancellationToken)
    {
        var characters = await handler.Handle(new GetCharactersQuery(), cancellationToken);
        return Results.Ok(characters);
    }
}
