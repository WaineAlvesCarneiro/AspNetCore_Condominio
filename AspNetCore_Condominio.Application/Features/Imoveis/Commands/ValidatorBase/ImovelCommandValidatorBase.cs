﻿using FluentValidation;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Commands.ValidatorBase;

public abstract class ImovelCommandValidatorBase<T> : AbstractValidator<T>
    where T : ICommandImovelBase
{
    protected void ConfigureCommonRules()
    {
        RuleFor(p => p.Bloco)
            .NotEmpty().WithMessage("Bloco é obrigatório")
            .Length(1, 10).WithMessage("O campo Bloco precisa ter entre 1 e 10 caracteres");

        RuleFor(p => p.Apartamento)
            .NotEmpty().WithMessage("Apartamento é obrigatório")
            .Length(1, 10).WithMessage("O campo Apartamento precisa ter entre 1 e 10 caracteres");

        RuleFor(p => p.BoxGaragem)
            .NotEmpty().WithMessage("Box Garagem é obrigatório")
            .Length(1, 10).WithMessage("O campo Box Garagem precisa ter entre 1 e 10 caracteres");
    }
}