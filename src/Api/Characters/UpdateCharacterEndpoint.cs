using Application.Characters.UpdateCharacter;
using Domain.Characters;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MinApiLib.Endpoints;

namespace Api.Characters;

[Tags("Characters")]
public record UpdateCharacter() : Put("/characters/{id}")
{
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
            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray());

            return Results.ValidationProblem(errors, statusCode: StatusCodes.Status422UnprocessableEntity);
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
