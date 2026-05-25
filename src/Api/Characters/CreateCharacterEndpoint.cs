using Application.Characters.CreateCharacter;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

public record CreateCharacter() : Post("/characters")
{
    public async Task<IResult> HandleAsync(
        [FromBody] CreateCharacterRequest body,
        [FromServices] CreateCharacterHandler handler,
        CancellationToken cancellationToken)
    {
        try
        {
            var character = await handler.Handle(
                new CreateCharacterCommand(body.Name, body.Race, body.PowerLevel, body.Affiliation),
                cancellationToken);

            return Results.Created($"/characters/{character.Id}", character);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status422UnprocessableEntity);
        }
    }
}

public sealed record CreateCharacterRequest(
    string Name,
    string Race,
    int PowerLevel,
    string Affiliation);
