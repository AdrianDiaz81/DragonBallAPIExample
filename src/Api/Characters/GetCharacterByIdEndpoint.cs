using Application.Characters.GetCharacterById;
using Domain.Characters;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record GetCharacterById() : Get("/characters/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
            .Produces<Character>(200)
            .ProducesProblem(404);

    public async Task<IResult> HandleAsync(
        int id,
        [FromServices] GetCharacterByIdHandler handler,
        CancellationToken cancellationToken)
    {
        var character = await handler.Handle(new GetCharacterByIdQuery(id), cancellationToken);
        return character is null ? Results.NotFound() : Results.Ok(character);
    }
}
