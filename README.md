Condomínio

Bem-vindo ao repositório do projeto AspNetCore_Condominio. Esta é a API back-end que suporta o sistema de gerenciamento de condomínios.

É um sistema projetado para Cloud permitindo que sejam cadastradas diversos condomínios como empresas, e diversos usuários.

Observação: Esse projeto tem na camada de apresentação endpoints com Controller e também Minimal API.

Descrição do Projeto:

Este projeto é a camada de serviços (API) que gerencia todas as informações do sistema de condomínio, incluindo:
Ao ser executado pela primeira vez o sistema cadastra automaticamente um usuário Admin com senha

Ao logar o Admin poderá:

Cadastro de empresa que será o condomínio, em seguida cadastra os usuários que serão utilizados no sistema.

Existe três perfis padrão de usuário que são:
Suporte - Admin
Síndico - administrador do condomínio
Porteiro - responsável pelos trabalhos diários da portaria

Quando logado com um usuário vinculado a uma empresa/condomínio o sistema somente irá mostrar os dados relacionados ao usuário logado de acordo com a empresa.

O usuário com perfil Síndico poderá cadastrar os imóveis e também inativar usuários. Somente o usuário com perfil suporte pode cadastrar ou excluir usuários.

Após cadastrar o imovel o usuário com perfil síndico pode cadastrar moradores e vincular ao seu imovel.

Link do Vídeo no Youtube:
[https://youtu.be/cFyPi0KkJH0](https://youtu.be/cFyPi0KkJH0)

Autenticação e Autorização:

Sistema de segurança utilizando JWT com Role para garantir que apenas usuários autorizados possam acessar a aplicação.


Tecnologias Utilizadas:

Backend:

C# e ASP.NET Core 8

API com Controller

Minimal API

DDD

CQRS

CORS

Entity Framework Core para acesso a dados e ORM

Swagger/OpenAPI para documentação interativa da API

JWT (JSON Web Tokens) para autenticação

Testes unitários

Tecnologias Utilizadas

Microsoft.AspNetCore.Mvc.Testing

Microsoft.EntityFrameworkCore.InMemory

Microsoft.NET.Test.Sdk

Moq

Testcontainers

xUnit

RabbitMQ

Worker

Docker para containerização da aplicação e do RabbitMQ

Mensageria:
RabbitMQ para comunicação assíncrona entre serviços e processamento de tarefas em segundo plano.
Sendo executado no Docker, o RabbitMQ é utilizado para enviar mensagens de eventos como cadastro de empresa, usuário e morador, que são processados um por um.
Worker Service para processar mensagens do RabbitMQ e executar tarefas em segundo plano como envio de notificações por e-mail, ao cadastrar empresa, usuário e morador.

Ao cadastrar uma empresa são informados os dados de SMTP para envio de e-mail, e o Worker Service utiliza essas informações para enviar um e-mail de boas vindas
	com as informações do cadastro para o usuário cadastrado.
		O mesmo acontece ao cadastrar um usuário ou morador, o sistema envia um e-mail de boas vindas com as informações do cadastro.


Banco de Dados:

SQL Server


Ferramentas:

Visual Studio 2022 ou Visual Studio Code

.NET SDK 8.0

Pré-requisitos

Antes de começar, certifique-se de que você tem as seguintes ferramentas instaladas:

.NET SDK 8.0

Um editor de código como Visual Studio ou Visual Studio Code

Um servidor de banco de dados SQL Server instalado para criar database e tabelas no banco de dados


Instalação e Configuração:

Siga os passos abaixo para configurar e executar a API localmente:

Clone o repositório:

Bash

git clone https://github.com/WaineAlvesCarneiro/AspNetCore_Condominio.git

Configuração do Banco de Dados:

Ao instalar o SQL Server defina o banco de dados localmente.

Em seguida execute os scripts disponíveis no projeto condomínio de Backend

AspNetCore_Condominio\AspNetCore_Condominio.Infrastructure\SQL_Server_Scrips


Execução da Aplicação:

Para rodar o projeto, abra o Visual Studio local na pasta do projeto e clique duas vezes no arquivo (.sln) após abrir o projeto pressionar F5.

A API estará rodando em 

https://localhost:44369/swagger/index.html

Para testar no Swagger ou frontend em Angular ou React

Como logar no sistema:

Usuário: Admin

Senha: 12345

Documentação da API

Você pode usar essa interface do swagger para testar os endpoints e entender a estrutura da API.

Ou se preferir usar a versão Web desenvolvida por mim em React basta baixar o projeto no repositório do GitHub e seguir o passo a passo descrito no README.md:

https://github.com/WaineAlvesCarneiro/react_condominio

Desenvolvido Waine Alves Carneiro


