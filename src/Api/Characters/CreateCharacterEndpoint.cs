using Application.Characters.CreateCharacter;
using Domain.Characters;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record CreateCharacter() : Post("/characters")
{
    protected override RouteHandlerBuilder Configure(RouteHandlerBuilder builder)
        => builder
            .Produces<Character>(201)
            .ProducesProblem(422)
            .ProducesProblem(400);

    public async Task<IResult> HandleAsync(
        [FromBody] CreateCharacterRequest body,
        [FromServices] CreateCharacterHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var character = await handler.Handle(
                new CreateCharacterCommand(
                    body.Name,
                    body.LastName,
                    body.Race,
                    body.PowerLevel,
                    body.Description,
                    body.Affiliation,
                    body.ImageUrl),
                cancellationToken);

            return Results.Created($"/characters/{character.Id}", character);
        }
        catch (ValidationException ex)
        {
            return ex.ToValidationProblem();
        }
    }
}

public sealed record CreateCharacterRequest(
    string Name,
    string LastName,
    string? Race,
    int PowerLevel,
    string? Description,
    string? Affiliation,
    string? ImageUrl);
