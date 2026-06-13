using Application.Characters.CreateCharacter;
using Domain.Characters;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
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
    string LastName,
    string? Race,
    int PowerLevel,
    string? Description,
    string? Affiliation,
    string? ImageUrl);
