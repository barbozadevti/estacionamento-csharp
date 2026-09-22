# Sistema de Estacionamento — Frontend

Dashboard operacional para gestão de um estacionamento: acompanha vagas disponíveis, registra entrada e saída de veículos e mantém um histórico de cobranças. Construído em React + TypeScript + Vite + Tailwind CSS, consumindo a API REST do backend em ASP.NET Core deste repositório.

## Stack

- [React 19](https://react.dev/) + [TypeScript](https://www.typescriptlang.org/)
- [Vite](https://vite.dev/) como bundler/dev server
- [Tailwind CSS v4](https://tailwindcss.com/) (via `@tailwindcss/vite`)

## Pré-requisitos

- Node.js 20+ e npm
- O backend (`estacionamento-csharp`) rodando em `http://localhost:5080` — ou ajuste a URL da API conforme abaixo.

## Rodando em desenvolvimento

```bash
npm install
npm run dev
```

A aplicação sobe por padrão em `http://localhost:5173`.

### Configurando a URL da API

Copie o arquivo de exemplo e ajuste se necessário:

```bash
cp .env.example .env
```

```
VITE_API_URL=http://localhost:5080/api
```

Se a variável não for definida, o cliente HTTP usa `http://localhost:5080/api` como fallback.

## Build de produção

```bash
npm run build
```

Gera os arquivos estáticos otimizados em `dist/`. Para pré-visualizar o build localmente:

```bash
npm run preview
```

## Lint

```bash
npm run lint
```

## Docker

A imagem faz build multi-stage (Node para compilar, nginx para servir os arquivos estáticos):

```bash
docker build -t estacionamento-frontend .
docker run -p 8080:80 estacionamento-frontend
```

A aplicação ficará disponível em `http://localhost:8080`. O `nginx.conf` inclui fallback de SPA (`try_files $uri /index.html`) para suportar rotas client-side no futuro.

> Nota: a URL da API é resolvida em tempo de build (`VITE_API_URL`). Para apontar a imagem para outro backend, ajuste a variável antes de rodar `npm run build` (ou rebuilde a imagem com `--build-arg`, caso essa capacidade seja adicionada ao Dockerfile).

## Estrutura do projeto

```
src/
  api/            cliente HTTP tipado para a API do backend
  components/     componentes de UI (cards de status, formulário, tabelas, toast, etc.)
  hooks/          hooks reutilizáveis (ex.: relógio para o tempo estacionado ao vivo)
  types/          tipos TypeScript compartilhados (contratos da API)
  utils/          formatação de moeda, datas e tempo decorrido
  App.tsx         composição do painel e orquestração de estado/polling
```

## Funcionalidades

- Cards de status: vagas disponíveis/total (com destaque visual quando lotado), preço inicial e preço por hora.
- Registro de entrada de veículo, com validação, normalização da placa (maiúsculas) e feedback de sucesso/erro.
- Lista de veículos estacionados com o tempo decorrido atualizado a cada segundo no cliente, e ação de registrar saída (mostrando o valor cobrado).
- Aba de histórico com os veículos que já saíram e o valor cobrado de cada um.
- Sincronização periódica com o servidor (poll a cada ~9s) além de atualização otimista após ações do usuário.
- Mensagem amigável quando o backend está indisponível, em vez de tela em branco.
