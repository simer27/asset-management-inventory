using AssetManagement.Inventory.API.DTOs.Item;
using FluentValidation;

namespace AssetManagement.Inventory.API.Validators.Item
{
    public class UpdateItemValidator : AbstractValidator<UpdateItemDto>
    {
        public UpdateItemValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("O nome do item é obrigatório.")
                .MaximumLength(150).WithMessage("O nome do item pode ter no máximo 150 caracteres.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("A descrição pode ter no máximo 500 caracteres.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(0)
                .WithMessage("A quantidade não pode ser negativa.");

            RuleFor(x => x.AreaId)
                .NotEmpty()
                .WithMessage("A área é obrigatória.");

            RuleFor(x => x.ValorMedio)
                .GreaterThanOrEqualTo(0)
                .When(x => x.ValorMedio.HasValue)
                .WithMessage("O valor médio não pode ser negativo.");
        }
    }
