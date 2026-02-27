-- IF EXISTS (SELECT name FROM sys.databases WHERE name = N'Condominio_api')
-- BEGIN
--     DROP DATABASE Condominio_api;
-- END

CREATE DATABASE Condominio_api
ON PRIMARY (
    NAME = Condominio_Data,
    FILENAME = 'D:\Projetos\Dados_API\Condominio_api.mdf',
    SIZE = 10MB,
    MAXSIZE = UNLIMITED,
    FILEGROWTH = 5MB
)
LOG ON (
    NAME = Condominio_Log,
    FILENAME = 'D:\Projetos\Dados_API\Condominio_api.ldf',
    SIZE = 5MB,
    MAXSIZE = 100MB,
    FILEGROWTH = 5MB
)
COLLATE Latin1_General_CI_AS;
GO