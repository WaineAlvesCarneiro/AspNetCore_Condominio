using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Utils;
using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.ValidatorBase;

public abstract class CommandValidatorBaseEmpresa<T> : AbstractValidator<T>
    where T : ICommandBaseEmpresa
{
    protected void ConfigureCommonRules(IEmpresaRepository repository)
    {
        RuleFor(p => p.RazaoSocial)
            .NotEmpty().WithMessage("Razão Social é obrigatória")
            .Length(3, 100).WithMessage("O campo Razão Social precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Fantasia)
            .NotEmpty().WithMessage("Fantasia é obrigatória")
            .Length(3, 100).WithMessage("O campo Fantasia precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Cnpj)
            .NotEmpty().WithMessage("Cnpj é obrigatório")
            .Must(CnpjValidator.IsValid).WithMessage("CNPJ informado é inválido")
            .MustAsync(async (command, cnpj, cancellation) =>
            {
                var cnpjLimpo = cnpj.Replace(".", "").Replace("-", "").Replace("/", "");
                var existe = await repository.ExisteCnpjAsync(cnpjLimpo, command.Id, cancellation);
                return !existe;
            }).WithMessage("Este CNPJ já está cadastrado para outra empresa.");

        RuleFor(p => p.TipoDeCondominio)
            .Must(x => Enum.IsDefined(typeof(TipoCondominio), x))
            .WithMessage("Tipo de Condomínio inválido. Selecione uma opção válida.");

        RuleFor(p => p.Nome)
            .NotEmpty().WithMessage("Nome do Responsável é obrigatório")
            .Length(3, 100).WithMessage("O campo Nome do Responsável precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Celular)
            .NotEmpty().WithMessage("Celular é obrigatório")
            .Length(11, 16).WithMessage("O campo Celular precisa ter entre 11 e 16 caracteres");

        RuleFor(p => p.Email)
            .NotEmpty().WithMessage("E-mail é obrigatório")
            .EmailAddress().WithMessage("Informe um e-mail válido")
            .Length(3, 100).WithMessage("O campo e-mail precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Host)
            .NotEmpty().WithMessage("Host é obrigatório")
            .Length(3, 100).WithMessage("O campo Host precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Porta)
            .NotEmpty().WithMessage("Porta é obrigatória")
            .GreaterThanOrEqualTo(1).WithMessage("A porta deve ser maior que zero");

        RuleFor(p => p.Cep)
            .NotEmpty().WithMessage("Cep é obrigatório")
            .Length(8, 8).WithMessage("O campo Cep precisa ter 8 caracteres");

        RuleFor(p => p.Uf)
            .NotEmpty().WithMessage("Uf é obrigatório")
            .Length(2, 2).WithMessage("O campo Uf precisa ter 2 caracteres");

        RuleFor(p => p.Cidade)
            .NotEmpty().WithMessage("Cidade é obrigatória")
            .Length(3, 100).WithMessage("O campo Cidade precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Endereco)
            .NotEmpty().WithMessage("Endereço é obrigatório")
            .Length(3, 100).WithMessage("O campo Endereço precisa ter entre 3 e 100 caracteres");

        RuleFor(p => p.Bairro)
            .NotEmpty().WithMessage("Bairro é obrigatório")
            .Length(3, 100).WithMessage("O campo Bairro precisa ter entre 3 e 100 caracteres");
    }
}