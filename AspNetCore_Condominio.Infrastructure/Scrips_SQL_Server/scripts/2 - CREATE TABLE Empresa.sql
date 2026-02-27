IF OBJECT_ID('[dbo].[Empresa]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Empresa] (
        [Id]                BIGINT IDENTITY(1,1) NOT NULL,
        [Ativo]             INT NOT NULL DEFAULT 1,
        [RazaoSocial]       VARCHAR(100) NOT NULL,
        [Fantasia]          VARCHAR(100) NOT NULL,
        [Cnpj]              VARCHAR(14) NOT NULL,
        [TipoDeCondominio]  INT         NOT NULL,
        [Nome]              VARCHAR(100) NOT NULL,
	    [Celular]           VARCHAR(16) NOT NULL,
	    [Telefone]          VARCHAR(15) NOT NULL,
	    [Email]             VARCHAR(100) NOT NULL,
        [Senha]             VARCHAR(100) NULL,
        [Host]              VARCHAR(100) NOT NULL,
        [Porta]             INT         NOT NULL,
	    [Cep]               VARCHAR(12) NOT NULL,
	    [Uf]                VARCHAR(2) NOT NULL,
	    [Cidade]            VARCHAR(100) NOT NULL,
	    [Endereco]          VARCHAR(100) NOT NULL,
        [Bairro]            VARCHAR(100) NOT NULL,
	    [Complemento]       VARCHAR(100) NOT NULL,
        [DataInclusao]      DATETIME2 NOT NULL CONSTRAINT DF_Empresa_Inclusao DEFAULT GETDATE(),
        [DataAlteracao]     DATETIME2 NULL,
        CONSTRAINT PK_Empresa PRIMARY KEY ([Id])
    );
END
GO