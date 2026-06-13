using FluentValidation;

namespace Application.Characters.CreateCharacter;

public sealed class CreateCharacterValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("El apellido es obligatorio.")
            .MaximumLength(100).WithMessage("El apellido no puede superar los 100 caracteres.");

        RuleFor(x => x.PowerLevel)
            .GreaterThanOrEqualTo(0).WithMessage("El nivel de poder no puede ser negativo.");

        When(x => x.Race is not null, () =>
            RuleFor(x => x.Race)
                .MaximumLength(100).WithMessage("La raza no puede superar los 100 caracteres."));

        When(x => x.Description is not null, () =>
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("La descripción no puede superar los 500 caracteres."));
    }
}
