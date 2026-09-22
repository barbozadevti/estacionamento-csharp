# Sistema de Estacionamento

Projeto iniciado como desafio de projeto do bootcamp da [DIO](https://www.dio.me/) ("Construindo um Sistema para um Estacionamento com C#") e evoluído, com uma sessão enxuta de [Lean Inception](docs/lean-inception.md), para um sistema completo: API REST em ASP.NET Core (Clean Architecture), banco de dados, frontend web em React e empacotamento com Docker.

O repositório guarda as três fases da evolução do projeto lado a lado:

| Versão | Pasta | Descrição |
|---|---|---|
| Desafio original (console) | [`src/EstacionamentoDIO.Console`](src/EstacionamentoDIO.Console) | Entrega original do desafio da DIO, em memória |
| Desktop (Windows Forms) | [`src/EstacionamentoDIO.Gui`](src/EstacionamentoDIO.Gui) | Evolução com interface gráfica e persistência em JSON |
| **Sistema completo (API + Web)** | [`backend/`](backend) e [`frontend/`](frontend) | Versão atual: arquitetura em camadas, banco de dados real e frontend web |

Veja o raciocínio por trás dessa evolução em [`docs/lean-inception.md`](docs/lean-inception.md).

## Sistema completo (API + Web)

### Arquitetura

```
backend/
├── EstacionamentoDIO.Domain/          # entidades, regras de negócio, exceções (sem dependências externas)
├── EstacionamentoDIO.Application/     # casos de uso, DTOs, interfaces
├── EstacionamentoDIO.Infrastructure/  # EF Core + SQLite, repositórios
├── EstacionamentoDIO.Api/             # controllers, Swagger, injeção de dependência
└── tests/
    ├── EstacionamentoDIO.Domain.Tests/  # testes de domínio e de casos de uso
    └── EstacionamentoDIO.Api.Tests/     # testes de integração (WebApplicationFactory)

frontend/
├── src/
│   ├── api/          # cliente HTTP tipado
│   ├── components/    # componentes da UI (cards de status, tabelas, formulário)
│   └── App.tsx         # painel operacional
└── Dockerfile
```

### Tecnologias

- **Backend**: C# / .NET 9, ASP.NET Core Web API, Entity Framework Core + SQLite, Swagger/OpenAPI, xUnit
- **Frontend**: React + TypeScript, Vite, Tailwind CSS
- **Infraestrutura**: Docker + Docker Compose

### Como rodar tudo com Docker (recomendado)

```bash
docker compose up --build
```

- API: http://localhost:5080/swagger
- Frontend: http://localhost:3000

### Como rodar localmente (sem Docker)

Backend:

```bash
cd backend
dotnet run --project EstacionamentoDIO.Api
```

A API sobe em `http://localhost:5080` (Swagger em `/swagger`) e aplica as migrations do EF Core automaticamente na primeira execução.

Frontend:

```bash
cd frontend
cp .env.example .env.local
npm install
npm run dev
```

### Como rodar os testes do backend

```bash
cd backend
dotnet test
```

## Desafio original: console (.NET)

```bash
dotnet run --project src/EstacionamentoDIO.Console
```

## Versão desktop (Windows Forms)

```bash
dotnet run --project src/EstacionamentoDIO.Gui
```

Testes do console/desktop (usa `EstacionamentoDIO.sln`, na raiz):

```bash
dotnet test
```
