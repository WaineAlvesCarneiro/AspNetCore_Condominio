using AspNetCore_Condominio.Domain.Entities;
using AspNetCore_Condominio.Domain.Enums;

namespace AspNetCore_Condominio.Domain.Tests.Entities;

public class EmpresaTests
{
    private Empresa CriarBase()
    {
        return new Empresa
        {
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "01.111.222/0001-02",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Nome Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Complemento = "Complemento",
            DataInclusao = DateTime.Now,
            DataAlteracao = null
        };
    }

    [Fact]
    public void Novo_DeveInicializarListaDeMoradoresVazia()
    {
        var dado = CriarBase();

        var dadoParaComparacao = new Empresa
        {
            RazaoSocial = "Razão Social",
            Fantasia = "Fantasia",
            Cnpj = "01.111.222/0001-02",
            TipoDeCondominio = (TipoCondominio)1,
            Nome = "Nome Responsável",
            Celular = "(11) 99999-9999",
            Telefone = "(11) 3333-3333",
            Email = "email@gmail.com",
            Cep = "01234-567",
            Uf = "SP",
            Cidade = "São Paulo",
            Endereco = "Rua Exemplo, 123",
            Complemento = "Complemento",
            DataInclusao = DateTime.Now,
            DataAlteracao = null
        };

        Assert.NotNull(dado);
        Assert.Empty(dado.Cnpj);

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
