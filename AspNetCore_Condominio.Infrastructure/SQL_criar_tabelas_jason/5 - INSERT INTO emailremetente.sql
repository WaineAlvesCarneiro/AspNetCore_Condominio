INSERT INTO public.emailremetente(username, senha, host, port)
-- Caso use Gmail o host é smtp.gmail.com e a port é 587
VALUES ('seu-email@gmail.com', 'senhas-app-do-email', 'smtp.gmail.com', 587)

-- Atenção: Se você estiver usando o Gmail, há algumas etapas adicionais que você precisa seguir para permitir que sua aplicação envie e-mails.
--O Gmail e outros provedores de e-mail modernos (como o Outlook) não permitem que aplicativos usem sua senha de conta principal diretamente para se conectar via SMTP.
--	Isso é uma medida de segurança para proteger sua conta.
--A Solução: Senhas de Aplicativo
--Para que o envio de e-mail funcione corretamente com o Gmail, você precisa gerar uma senha de aplicativo (App Password).
--	Esta é uma senha única, de 16 dígitos, que você cria especificamente para o seu aplicativo e que substitui a sua senha principal no código.
--Como Gerar uma Senha de Aplicativo no Gmail:
--Acesse sua Conta do Google.
--No menu de navegação à esquerda, selecione Segurança.
--Em "Como fazer login no Google", selecione Verificação em duas etapas. Se essa opção estiver desativada, você precisará ativá-la primeiro.
--Role para baixo e selecione Senhas de app.
--Você será solicitado a fazer login novamente.
--Na tela de "Senhas de app", no menu suspenso Selecionar app, escolha Correio.
--	No menu suspenso Selecionar dispositivo, escolha Outro (Nome personalizado) e digite um nome para sua aplicação (ex: "Minha API Condomínio").
--Clique em Gerar. O Gmail mostrará uma senha de 16 dígitos. Copie essa senha e guarde-a em um lugar seguro.