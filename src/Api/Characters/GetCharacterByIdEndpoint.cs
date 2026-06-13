using Application.Characters.GetCharacterById;
using Domain.Characters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record GetCharacterById() : Get("/characters/{id}")
{
    public async Task<IResult> HandleAsync(
        int id,
        [FromServices] GetCharacterByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var character = await handler.Handle(new GetCharacterByIdQuery(id), cancellationToken);
        return character is null ? Results.NotFound() : Results.Ok(character);
    }
}
