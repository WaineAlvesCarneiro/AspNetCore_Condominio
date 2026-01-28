using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class EmpresaTests
{
    private DateTime DateAtual = DateTime.UtcNow;

    private Empresa CriarBase()
    {
        return new Empresa
        {
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "01111222000102",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Nome Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Senha = "SenhaSegura123!",
            Host = "smtp.email.com",
            Porta = 587,
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateAtual,
            DataAlteracao = DateAtual
        };
    }

    [Fact]
    public void Novo_DeveInicializarListaDeEmpresasVazia()
    {
        var dado = CriarBase();

        var dadoParaComparacao = new Empresa
        {
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "01111222000102",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Nome Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Senha = "SenhaSegura123!",
            Host = "smtp.email.com",
            Porta = 587,
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Bairro = "Pq Amazônia",
            Complemento = "Complemento",
            DataInclusao = DateAtual,
            DataAlteracao = DateAtual
        };

        Assert.NotNull(dado);
        Assert.IsType<Empresa>(dado);
        Assert.Equal(dadoParaComparacao.RazaoSocial, dado.RazaoSocial);
        Assert.Equal(dadoParaComparacao.Fantasia, dado.Fantasia);
        Assert.Equal(dadoParaComparacao.Cnpj, dado.Cnpj);

        foreach (var prop in dado.GetType().GetProperties())
        {
            var valorEsperado = prop.GetValue(dadoParaComparacao);
            var valorAtual = prop.GetValue(dado);
            Assert.Equal(valorEsperado, valorAtual);
        }

        foreach (var prop in dado.GetType().GetProperties())
        {
            Assert.NotNull(prop.GetValue(dado));
        }
        foreach (var prop in dado.GetType().GetProperties())
        {
            if (prop.PropertyType == typeof(string))
            {
                Assert.False(string.IsNullOrWhiteSpace((string)prop.GetValue(dado)!));
            }
        }
    }
}
