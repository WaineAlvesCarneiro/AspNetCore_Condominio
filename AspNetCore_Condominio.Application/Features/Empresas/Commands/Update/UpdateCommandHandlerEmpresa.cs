using AspNetCore_Condominio.Application.DTOs;
using AspNetCore_Condominio.Application.Helpers; // Certifique-se de apontar para o seu EncryptionHelper
using AspNetCore_Condominio.Domain.Common;
using AspNetCore_Condominio.Domain.Enums;
using AspNetCore_Condominio.Domain.Interfaces;
using AspNetCore_Condominio.Domain.Repositories;
using AspNetCore_Condominio.Domain.Repositories.Auth;
using MediatR;

namespace AspNetCore_Condominio.Application.Features.Empresas.Commands.Update;

public record UpdateCommandHandlerEmpresa(
    IEmpresaRepository repository,
    IAuthUserRepository authUserRepository,
    IMensageriaService mensageriaService)
    : IRequestHandler<UpdateCommandEmpresa, Result<EmpresaDto>>
{
    private readonly IEmpresaRepository _repository = repository;
    private readonly IAuthUserRepository _authUserRepository = authUserRepository;

    public async Task<Result<EmpresaDto>> Handle(UpdateCommandEmpresa request, CancellationToken cancellationToken)
    {
        var dadoToUpdate = await _repository.GetByIdAsync(request.Id);
        if (dadoToUpdate == null)
            return Result<EmpresaDto>.Failure("Empresa não encontrada.");

        bool statusMudouParaInativo = dadoToUpdate.Ativo == TipoEmpresaAtivo.Ativo && request.Ativo != TipoEmpresaAtivo.Ativo;

        // --- SEGURANÇA: Tratamento da Senha SMTP ---
        // Se a senha enviada for diferente da atual ou se você quiser garantir que ela
        // sempre seja criptografada ao salvar:
        if (!string.IsNullOrEmpty(request.Senha))
        {
            // Criptografamos antes de atribuir à entidade
            dadoToUpdate.Senha = EncryptionHelper.Encrypt(request.Senha);
        }

        dadoToUpdate.Ativo = request.Ativo;
        dadoToUpdate.RazaoSocial = request.RazaoSocial;
        dadoToUpdate.Fantasia = request.Fantasia;
        dadoToUpdate.Cnpj = request.Cnpj.Replace(".", "").Replace("-", "").Replace("/", ""); // Sanitização básica
        dadoToUpdate.TipoDeCondominio = request.TipoDeCondominio;
        dadoToUpdate.Nome = request.Nome;
        dadoToUpdate.Celular = request.Celular;
        dadoToUpdate.Telefone = request.Telefone;
        dadoToUpdate.Email = request.Email;
        dadoToUpdate.Host = request.Host;
        dadoToUpdate.Porta = request.Porta;
        dadoToUpdate.Cep = request.Cep;
        dadoToUpdate.Uf = request.Uf;
        dadoToUpdate.Cidade = request.Cidade;
        dadoToUpdate.Endereco = request.Endereco;
        dadoToUpdate.Bairro = request.Bairro;
        dadoToUpdate.Complemento = request.Complemento;
        dadoToUpdate.DataAlteracao = DateTime.Now; // Usar a data atual na alteração

        await _repository.UpdateAsync(dadoToUpdate);

        // --- Lógica de Cascata para Usuários ---
        if (statusMudouParaInativo)
        {
            var usuarios = await _authUserRepository.GetByEmpresaIdAsync(dadoToUpdate.Id);
            foreach (var usuario in usuarios)
            {
                usuario.EmpresaAtiva = request.Ativo;
                usuario.DataAlteracao = DateTime.Now;
                await _authUserRepository.UpdateAsync(usuario);
            }
        }

        var dto = new EmpresaDto
        {
            Id = dadoToUpdate.Id,
            Ativo = dadoToUpdate.Ativo,
            RazaoSocial = dadoToUpdate.RazaoSocial,
            Fantasia = dadoToUpdate.Fantasia,
            Cnpj = dadoToUpdate.Cnpj,
            TipoDeCondominio = dadoToUpdate.TipoDeCondominio,
            Nome = dadoToUpdate.Nome,
            Celular = dadoToUpdate.Celular,
            Telefone = dadoToUpdate.Telefone ?? "",
            Email = dadoToUpdate.Email,
            // Senha = null, // NUNCA retorne a senha para o React por questões de segurança
            Host = dadoToUpdate.Host,
            Porta = dadoToUpdate.Porta,
            Cep = dadoToUpdate.Cep,
            Uf = dadoToUpdate.Uf,
            Cidade = dadoToUpdate.Cidade,
            Endereco = dadoToUpdate.Endereco,
            Bairro = dadoToUpdate.Bairro,
            Complemento = dadoToUpdate.Complemento,
            DataInclusao = dadoToUpdate.DataInclusao,
            DataAlteracao = dadoToUpdate.DataAlteracao
        };

        // MENSAGERIA REAL (RabbitMQ)
        // O Handler não espera o e-mail ser enviado, ele apenas publica na fila
        try
        {
            string corpoEmail = $@"
                <h3>Sistema de Condomínio!</h3>
                <p>Empresa alterada com sucesso.</p>";

            await mensageriaService.EnviarEmailAsync(
                dadoToUpdate.Email,
                "Os dados foram alterados",
                corpoEmail,
                dadoToUpdate.Id
            );
        }
        catch (Exception ex)
        {
            // Importante: Se o RabbitMQ falhar, o empresa já foi alterado no banco.
            // No mercado, usamos padrões como 'Outbox Pattern' para lidar com isso,
            // mas por enquanto, apenas logar o erro é o suficiente.
            Console.WriteLine($"Erro ao publicar no RabbitMQ: {ex.Message}");
        }

        return Result<EmpresaDto>.Success(dto, "Empresa atualizada com sucesso.");
    }
}