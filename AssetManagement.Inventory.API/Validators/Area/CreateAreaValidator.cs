using AssetManagement.Inventory.API.DTOs.Area;
using FluentValidation;

namespace AssetManagement.Inventory.API.Validators.Area
{
    public class CreateAreaValidator : AbstractValidator<CreateAreaDto>
    {
        public CreateAreaValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da área é obrigatório.")
                .MaximumLength(150).WithMessage("O nome da área pode ter no máximo 150 caracteres.");
        }
    }
}
