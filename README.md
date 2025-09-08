Asp Net Core Condominio

Bem-vindo ao repositório do projeto AspNetCore_Condominio. Esta é a API back-end que suporta o sistema de gerenciamento de condomínios, desenvolvida com o poderoso framework ASP.NET Core 8.

Observação: Esse projeto tem duas camadas de apresentação:

Uma camada de apresentação com endpoints utilizando Controller

Outra camada e apresentação com endpoints utilizando Minimal API

Que permite que ao executar localmente possa ser utilizada qualquer uma das duas camadas de apresentação com Controller ou Minimal API

Descrição do Projeto

Este projeto é a camada de serviços (API) que gerencia todas as informações do sistema de condomínio, incluindo:

Gerenciamento de Moradores e Unidades: Endpoints para o ciclo de vida completo de moradores e apartamentos.

Ao cadastrar ou editar os dados do morador a API envia um email para o morador.

Validações de campos e ao excluir um imóvel com morador vinculado o sistema exibe uma notificação.

Autenticação e Autorização: Sistema de segurança utilizando JWT para garantir que apenas usuários autorizados possam acessar a aplicação.

Tecnologias Utilizadas

Backend:

C# e ASP.NET Core 8

API com Controller (com paginação)

Minimal API (com paginação)

DDD

CQRS

Entity Framework Core para acesso a dados e ORM

Swagger/OpenAPI para documentação interativa da API

JWT (JSON Web Tokens) para autenticação

Banco de Dados:

PostgreSQL

Ferramentas:

Visual Studio 2022 ou Visual Studio Code

.NET SDK 8.0

Pré-requisitos

Antes de começar, certifique-se de que você tem as seguintes ferramentas instaladas:

.NET SDK 8.0

Um editor de código como Visual Studio ou Visual Studio Code

Um servidor de banco de dados PostgreSQL instalado juntamente com pgAdmin 4 para criar database e tabelas no banco de dados

Instalação e Configuração

Siga os passos abaixo para configurar e executar a API localmente:

Clone o repositório:

Bash

git clone https://github.com/WaineAlvesCarneiro/AspNetCore_Condominio.git

cd AspNetCore_Condominio

Configuração do Banco de Dados:

Ao instalar o PostgreSQL defina:

username=postgres

password=postgres

Em seguida execute os scripts disponíveis no projeto condomínio de Backend

https://github.com/WaineAlvesCarneiro/AspNetCore_Condominio/tree/master/AspNetCore_Condominio.Infrastructure/SQL_criar_tabelas_jason

Execução da Aplicação:

Para rodar o projeto, execute o seguinte comando:

Bash

dotnet run

Ou, se estiver usando o Visual Studio, basta abrir o arquivo de solução (.sln) e pressionar F5.

A API estará rodando em 

https://localhost:44369/swagger/index.html

Para testar no Swagger ou frontend em Angular use:

Usuário: admin

Senha: 12345

Documentação da API

Você pode usar essa interface do swagger para testar os endpoints e entender a estrutura da API.

Ou se preferir usar a versão Web desenvolvida por mim em Angular basta baixar o projeto angular_condominio no repositório do GitHub e seguir o passo a passo descrito no README.md:

https://github.com/WaineAlvesCarneiro/angular_condominio

Desenvolvido Waine Alves Carneiro
