using FluentValidation;

namespace Application.Characters.CreateCharacter;

public sealed class CreateCharacterValidator : AbstractValidator<CreateCharacterCommand>
{
    public CreateCharacterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre es obligatorio.")
            .MaximumLength(100).WithMessage("El nombre no puede superar los 100 caracteres.");

        RuleFor(x => x.Race)
            .NotEmpty().WithMessage("La raza es obligatoria.")
            .MaximumLength(50).WithMessage("La raza no puede superar los 50 caracteres.");

        RuleFor(x => x.PowerLevel)
            .GreaterThan(0).WithMessage("El nivel de poder debe ser mayor que 0.");

        RuleFor(x => x.Affiliation)
            .NotEmpty().WithMessage("La afiliación es obligatoria.")
            .MaximumLength(100).WithMessage("La afiliación no puede superar los 100 caracteres.");
    }
}
