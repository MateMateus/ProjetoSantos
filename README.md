*✝️ Santos Católicos - Enciclopédia Digital*
Uma aplicação web Fullstack para catalogar, buscar e gerenciar informações sobre Santos da Igreja Católica, incluindo suas histórias, dias de festa e padroados.

📸 Preview
[(https://santospedia.netlify.app/)](https://santospedia.netlify.app/)

🚀 Sobre o Projeto
Este projeto foi desenvolvido para facilitar o acesso à hagiografia (história dos santos) de forma moderna e responsiva. O sistema conta com uma área pública de consulta e uma área administrativa protegida para gerenciamento do conteúdo.

Funcionalidades Principais

Público:
🕊️ Listagem de Santos com fotos e resumos.
🔍 Busca em tempo real por nome ou padroeiro (ex: "Protetor dos animais").
📖 Página de detalhes com biografia completa.

Administrativo (Painel Admin):
🔒 Autenticação de usuários.
➕ Cadastro de novos santos (Create).
✏️ Edição e Exclusão de registros (Update/Delete).
🛠️ Tecnologias e Arquitetura

O projeto utiliza uma arquitetura Client-Server desacoplada, hospedada em serviços de nuvem gratuitos.
Frontend (Cliente)
Hospedagem: Netlify
Tecnologias: HTML5, CSS3 (Responsivo), JavaScript (Vanilla/ES6+).
Destaques: Uso de fetch API para comunicação assíncrona com o backend.
Backend (Servidor API)
Hospedagem: Render
Tecnologias: Node.js, Express (API RESTful).
Segurança: Configuração de CORS para permitir requisições apenas do domínio do Frontend.
Banco de Dados
SGBD: PostgreSQL.
Hospedagem: Render (PostgreSQL Instance) / Migração planejada para Supabase para persistência de longo prazo.

🧩 Como Funciona a Integração
O desafio técnico principal deste projeto foi a integração entre dois ambientes de nuvem distintos:
O Frontend (Netlify) faz requisições HTTP para a API.
A API (Render) processa a requisição, aplica regras de negócio e consulta o Banco de Dados.
O PostgreSQL retorna os dados, que são enviados de volta ao Frontend em formato JSON.
Nota sobre CORS: Foi necessário configurar o Cross-Origin Resource Sharing (CORS) no servidor Node.js para aceitar as chamadas vindas do domínio do Netlify, garantindo a segurança da comunicação.

💻 Como Rodar Localmente
Pré-requisitos
Node.js instalado.
Git instalado.

1. Clonar o Repositório
git clone [https://github.com/SEU-USUARIO/santos-catolicos.git](https://github.com/SEU-USUARIO/santos-catolicos.git)
cd santos-catolicos

2. Configurar o Backend
cd backend
npm install
# Crie um arquivo .env com as credenciais do banco
# DATABASE_URL=postgres://user:pass@host:port/db
# PORT=3000
npm start

3. Configurar o Frontend
Vá até a pasta do frontend.
Abra o arquivo de configuração da API (ex: js/api.js).
Altere a URL base para http://localhost:3000.
Abra o index.html no navegador (ou use o Live Server do VS Code).

🚧 Melhorias Futuras
[ ] Migração do Banco de Dados para Supabase (para evitar a expiração do plano gratuito do Render).
[ ] Implementação de Upload de Imagens (atualmente usa URLs externas).
[ ] Modo Escuro (Dark Mode).

🤝 Contribuição
Contribuições são bem-vindas! Sinta-se à vontade para abrir uma issue ou enviar um Pull Request.

<p align="center">
Desenvolvido com fé e código 💜
</p>
