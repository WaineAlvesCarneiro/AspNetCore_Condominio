using AspNetCore_Condominio.Domain.Enums;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;

public abstract class CommandValidatorBaseEmpresa<T> : AbstractValidator<T>
    where T : ICommandBaseEmpresa
{
    protected void ConfigureCommonRules()
    {
        RuleFor(p => p.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .Length(2, 70).WithMessage("O campo Razão Social precisa ter entre 2 e 70 caracteres");

        RuleFor(p => p.Fantasia)
            .NotEmpty().WithMessage("Fantasia é obrigatória")
            .Length(2, 50).WithMessage("O campo Fantasia precisa ter entre 2 e 50 caracteres");

        RuleFor(p => p.Cnpj)
            .NotEmpty().WithMessage("Cnpj é obrigatório")
            .Length(14, 14).WithMessage("O campo Cnpj precisa ter 14 caracteres");

        RuleFor(p => p.TipoDeCondominio)
            .Must(x => Enum.IsDefined(typeof(TipoCondominio), x))
            .WithMessage("Tipo de Condomínio inválido. Selecione uma opção válida.");

        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("Nome do Responsável é obrigatório")
            .Length(2, 50).WithMessage("O campo Nome do Responsável precisa ter entre 2 e 50 caracteres");

        RuleFor(p => p.Celular)
            .NotEmpty().WithMessage("Celular é obrigatório")
            .Length(11, 16).WithMessage("O campo Celular precisa ter entre 11 e 16 caracteres");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("Email é obrigatório")
            .EmailAddress().WithMessage("Informe um email válido");

        RuleFor(p => p.Host)
            .NotEmpty().WithMessage("Host é obrigatório")
            .Length(2, 50).WithMessage("O campo Host do Responsável precisa ter entre 2 e 50 caracteres");

        RuleFor(p => p.Porta)
            .NotEmpty().WithMessage("Porta é obrigatória")
            .InclusiveBetween(1, 65535).WithMessage("O campo Porta precisa estar entre 1 e 65535");

        RuleFor(p => p.Cep)
            .NotEmpty().WithMessage("Cep é obrigatório")
            .Length(8, 8).WithMessage("O campo Cep precisa ter 8 caracteres");

        RuleFor(p => p.Uf)
            .NotEmpty().WithMessage("Uf é obrigatório")
            .Length(2, 2).WithMessage("O campo Uf precisa ter 2 caracteres");

        RuleFor(p => p.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .Length(2, 50).WithMessage("O campo Cidade precisa ter entre 2 e 50 caracteres");

        RuleFor(p => p.Endereco)
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .Length(2, 50).WithMessage("O campo Endereço precisa ter entre 2 e 50 caracteres");

        RuleFor(p => p.Bairro)
            .NotEmpty().WithMessage("Bairro é obrigatório")
            .Length(2, 50).WithMessage("O campo Bairro precisa ter entre 2 e 50 caracteres");
    }
}