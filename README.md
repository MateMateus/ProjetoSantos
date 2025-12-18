

# ✝️ Santos Católicos — Enciclopédia Digital

> **Status:** Online 🟢

Uma aplicação web **Fullstack** para catalogar, buscar e gerenciar informações sobre Santos da Igreja Católica, preservando suas histórias, dias de festa e padroados.

O objetivo é facilitar o acesso à hagiografia (história dos santos) através de uma interface moderna, responsiva e de fácil navegação.

-----

## 📸 Preview

Acesse o projeto online: **[santospedia.netlify.app](https://santospedia.netlify.app/)**

*(Substitua este link acima por um print real da sua tela)*

-----

## 🚀 Funcionalidades

O sistema é dividido em dois módulos de acesso:

### 🕊️ Área Pública

  * **Catálogo Visual:** Listagem de santos com fotos e resumos biográficos.
  * **Busca Inteligente:** Pesquisa em tempo real por nome ou padroeiro (ex: *"Protetor dos animais"*).
  * **Detalhes:** Página dedicada com a biografia completa e datas comemorativas.

### 🔒 Área Administrativa (Painel Admin)

  * **Autenticação:** Login seguro para administradores.
  * **Gestão de Conteúdo:**
      * ➕ **Create:** Cadastro de novos santos.
      * ✏️ **Update:** Edição de informações existentes.
      * ❌ **Delete:** Remoção de registros.

-----

## 🛠️ Tecnologias e Arquitetura

O projeto utiliza uma arquitetura **Client-Server desacoplada**, onde o Frontend e o Backend operam em ambientes distintos, comunicando-se via API REST.

### 🎨 Frontend (Cliente)

  * **Hospedagem:** [Netlify](https://www.netlify.com/)
  * **Linguagens:** HTML5, CSS3 (Responsivo), JavaScript (Vanilla/ES6+).
  * **Comunicação:** Utiliza `Fetch API` para consumir os dados do backend.

### ⚙️ Backend (Servidor API)

  * **Hospedagem:** [Render](https://render.com/)
  * **Runtime:** Node.js
  * **Framework:** Express (API RESTful)
  * **Segurança:** Configuração de **CORS** restritivo para aceitar apenas requisições do domínio do Netlify.

### 🗄️ Banco de Dados

  * **SGBD:** PostgreSQL
  * **Hospedagem:** Render (PostgreSQL Instance)

> **Nota sobre Integração:** O maior desafio técnico foi orquestrar a comunicação segura entre o Netlify (Front) e o Render (Back/DB), garantindo que as requisições Cross-Origin (CORS) fossem processadas corretamente.

-----

## 💻 Como Rodar Localmente

Siga os passos abaixo para executar o projeto em sua máquina.

### Pré-requisitos

  * [Node.js](https://nodejs.org/) instalado.
  * [Git](https://git-scm.com/) instalado.
  * PostgreSQL instalado localmente (opcional, caso não conecte no banco da nuvem).

### 1\. Clonar o Repositório

```bash
git clone https://github.com/SEU-USUARIO/santos-catolicos.git
cd santos-catolicos
```

### 2\. Configurar o Backend

Navegue até a pasta do servidor e instale as dependências:

```bash
cd backend
npm install
```

Crie um arquivo `.env` na raiz da pasta `backend` com suas credenciais (exemplo):

```env
DATABASE_URL=sua_string_conexao_postgres
PORT=3000
```

Inicie o servidor:

```bash
npm start
# O servidor rodará em http://localhost:3000
```

### 3\. Configurar o Frontend

1.  Vá até a pasta do frontend (raiz ou `frontend/`).
2.  Abra o arquivo de configuração da API (ex: `js/api.js` ou `script.js`).
3.  Altere a URL base da API de produção para local:

<!-- end list -->

```javascript
// const API_URL = "https://sua-api-no-render.com";
const API_URL = "http://localhost:3000";
```

4.  Abra o arquivo `index.html` no seu navegador ou use uma extensão como **Live Server** no VS Code.

-----

## 🚧 Roadmap e Melhorias Futuras

  - [ ] **Migração de Banco de Dados:** Migrar do Render para o **Supabase** para garantir persistência a longo prazo e evitar limitações do plano gratuito.
  - [ ] **Upload de Imagens:** Implementar upload real de arquivos (atualmente utiliza URLs de imagens externas).
  - [ ] **Dark Mode:** Implementar tema escuro para melhor acessibilidade noturna.

-----

## 🤝 Contribuição

Contribuições são muito bem-vindas\!

1.  Faça um Fork do projeto.
2.  Crie uma Branch para sua Feature (`git checkout -b feature/NovaFeature`).
3.  Faça o Commit (`git commit -m 'Adicionando nova feature'`).
4.  Faça o Push (`git push origin feature/NovaFeature`).
5.  Abra um Pull Request.

-----

\<p align="center"\>
Desenvolvido com fé e código 💜
\</p\>
