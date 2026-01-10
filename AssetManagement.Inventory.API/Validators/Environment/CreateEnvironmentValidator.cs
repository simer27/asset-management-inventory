using AssetManagement.Inventory.API.DTOs.EnvironmentDto;
using FluentValidation;

namespace AssetManagement.Inventory.API.Validators.Environment
{
    public class CreateEnvironmentValidator : AbstractValidator<CreateEnvironmentDto>
    {
        public CreateEnvironmentValidator()
        {
            RuleFor(x => x.Nome)
                .NotEmpty().WithMessage("O nome do ambiente é obrigatório.")
                .MaximumLength(150);

            RuleFor(x => x.Descricao)
                .MaximumLength(500);

            RuleFor(x => x.Images)
                .Must(images => images == null || images.Count <= 5)
                .WithMessage("Você pode enviar no máximo 5 imagens.");
        }
    }
}
