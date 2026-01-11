using AssetManagement.Inventory.API.DTOs.Area;
using FluentValidation;

namespace AssetManagement.Inventory.API.Validators.Area
{
    public class UpdateAreaValidator : AbstractValidator<UpdateAreaDto>
    {
        public UpdateAreaValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome da área é obrigatório.")
                .MaximumLength(150).WithMessage("O nome da área pode ter no máximo 150 caracteres.");
        }
    }
}
