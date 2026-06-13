using FluentValidation;

namespace Application.Characters.UpdateCharacter;

public sealed class UpdateCharacterValidator : AbstractValidator<UpdateCharacterCommand>
{
    public UpdateCharacterValidator()
    {
        When(x => x.Name is not null, () =>
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("El nombre no puede estar vacío.")
                .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres."));

        When(x => x.LastName is not null, () =>
            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("El apellido no puede estar vacío.")
                .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres."));

        When(x => x.PowerLevel.HasValue, () =>
            RuleFor(x => x.PowerLevel!.Value)
                .GreaterThanOrEqualTo(0).WithMessage("El nivel de poder no puede ser negativo."));

        When(x => x.Race is not null, () =>
            RuleFor(x => x.Race)
                .MaximumLength(100).WithMessage("La raza no puede superar los 100 caracteres."));

        When(x => x.Description is not null, () =>
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres."));
    }
}
