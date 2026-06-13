using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Api.Characters;

internal static class ValidationHelper
{
    internal static IResult ToValidationProblem(this ValidationException ex) =>
        Results.ValidationProblem(
            ex.Errors
              .GroupBy(e => e.PropertyName)
              .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage).ToArray()),
            statusCode: StatusCodes.Status422UnprocessableEntity);
}
