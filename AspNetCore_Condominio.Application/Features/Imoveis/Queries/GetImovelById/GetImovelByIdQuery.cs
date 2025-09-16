﻿using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Domain.Common;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Imoveis.Queries.GetImovelById;

public record GetImovelByIdQuery(int Id) : IRequest<Result<ImovelDto>>;