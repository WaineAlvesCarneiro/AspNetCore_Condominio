 IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Condominio_api')
 BEGIN
     DROP DATABASE Condominio_api;
 END

CREATE DATABASE Condominio_api
ON PRIMARY (
    NAME = Condominio_Data,
    FILENAME = 'C:\SQLData\Condominio_api.mdf', -- Defina o caminho real aqui
    SIZE = 10MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 5MB
)
LOG ON (
    NAME = Condominio_Log,
    FILENAME = 'C:\SQLData\Condominio_api_log.ldf',
    SIZE = 5MB,
    MAXSIZE = 250MB,
    FILEGROWTH = 5MB
)
COLLATE Latin1_General_CI_AS;
GO


USE [Condominio_api]
GO

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


IF OBJECT_ID('[dbo].[AuthUsers]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[AuthUsers] (
        [Id]              UNIQUEIDENTIFIER CONSTRAINT PK_AuthUsers PRIMARY KEY DEFAULT NEWSEQUENTIALID(),
        [Ativo]           INT NOT NULL DEFAULT 1,
        [EmpresaAtiva]    INT NOT NULL DEFAULT 1,
        [EmpresaId]       BIGINT NULL,
        [UserName]        VARCHAR(100) NOT NULL CONSTRAINT UQ_AuthUsers_UserName UNIQUE,
        [Email]           VARCHAR(100) NOT NULL,
        [PasswordHash]    NVARCHAR(MAX) NOT NULL,
        [Role]            INT NOT NULL,
        [PrimeiroAcesso]  BIT NOT NULL,
        [DataInclusao]    DATETIME2 NOT NULL CONSTRAINT DF_AuthUsers_Inclusao DEFAULT GETDATE(),
        [DataAlteracao]   DATETIME2 NULL,
    );
END
GO

IF OBJECT_ID('[dbo].[Imovel]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Imovel] (
        [Id]          BIGINT IDENTITY(1,1) NOT NULL,
        [Bloco]       VARCHAR(100) NOT NULL,
        [Apartamento] VARCHAR(100) NOT NULL,
        [BoxGaragem]  VARCHAR(100) NOT NULL,
        [EmpresaId]   BIGINT NOT NULL,
        
        CONSTRAINT PK_Imovel PRIMARY KEY ([Id])
    );
END
GO

IF OBJECT_ID('[dbo].[Morador]', 'U') IS NULL
BEGIN
    CREATE TABLE [dbo].[Morador] (
        [Id]             BIGINT IDENTITY(1,1) NOT NULL,
        [Nome]           VARCHAR(100) NOT NULL,
        [Celular]        VARCHAR(16) NOT NULL,
        [Email]          VARCHAR(100) NOT NULL,
        [IsProprietario] BIT NOT NULL,
        [DataEntrada]    DATE NOT NULL,
        [DataSaida]      DATE NULL,
        [DataInclusao]   DATETIME2 NOT NULL CONSTRAINT DF_Morador_Inclusao DEFAULT GETDATE(),
        [DataAlteracao]  DATETIME2 NULL,
        [ImovelId]       BIGINT NOT NULL,
        [EmpresaId]      BIGINT NOT NULL,

        CONSTRAINT PK_Morador PRIMARY KEY ([Id]),
    );
END
GO