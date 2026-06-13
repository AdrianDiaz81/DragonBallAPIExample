using Application.Characters.UpdateCharacter;
using Domain.Characters;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record UpdateCharacter() : Put("/characters/{id}")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
            .Produces<Character>(200)
            .ProducesProblem(404)
            .ProducesProblem(422)
            .ProducesProblem(400);

    public async Task<IResult> HandleAsync(
        int id,
        [FromBody] UpdateCharacterRequest body,
        [FromServices] UpdateCharacterHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var character = await handler.Handle(
                new UpdateCharacterCommand(
                    id,
                    body.Name,
                    body.LastName,
                    body.Race,
                    body.PowerLevel,
                    body.Description,
                    body.Affiliation,
                    body.ImageUrl),
                cancellationToken);

            return character is null ? Results.NotFound() : Results.Ok(character);
        }
        catch (ValidationException ex)
        {
            return ex.ToValidationProblem();
        }
    }
}

public sealed record UpdateCharacterRequest(
    string? Name = null,
    string? LastName = null,
    string? Race = null,
    int? PowerLevel = null,
    string? Description = null,
    string? Affiliation = null,
    string? ImageUrl = null);
