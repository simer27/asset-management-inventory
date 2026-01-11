using AssetManagement.Inventory.API.DTOs.DocumentDto;
using FluentValidation;

namespace AssetManagement.Inventory.API.Validators.ProofDocument
{
    public class UploadDocumentValidator : AbstractValidator<UploadDocumentDto>
    {
        public UploadDocumentValidator()
        {
            RuleFor(x => x.File)
                .NotNull().WithMessage("O arquivo é obrigatório.")
                .Must(f => f.Length > 0).WithMessage("O arquivo não pode estar vazio.")
                .Must(f => f.Length <= 10 * 1024 * 1024)
                .WithMessage("O arquivo pode ter no máximo 10MB.");

            RuleFor(x => x.Type)
                .IsInEnum()
                .WithMessage("Tipo de documento inválido.");
        }
    }
}
