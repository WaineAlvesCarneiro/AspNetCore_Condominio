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