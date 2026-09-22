# Lean Inception — Sistema de Estacionamento

Versão enxuta (solo, aplicada a este projeto) do processo de Lean Inception, para justificar por que o sistema evoluiu do desafio original (CRUD simples em memória) para uma arquitetura em camadas com API e frontend web.

## 1. Objetivo do produto

Ir além do desafio original da DIO e entregar um sistema de gestão de estacionamento utilizável de verdade por um operador no dia a dia, demonstrando arquitetura em camadas (Domain/Application/Infrastructure/API), persistência real e uma interface web responsiva.

## 2. Personas

- **Operador do estacionamento** (usuário primário): registra entradas e saídas de veículos, consulta vagas disponíveis, informa o valor a cobrar do cliente na saída.
- **Gestor do estacionamento** (usuário secundário): acompanha ocupação e quanto foi arrecadado no período.

## 3. Jornada do operador

1. Abre o sistema no início do turno → vê quantas vagas estão disponíveis.
2. Veículo chega → digita a placa → sistema registra a entrada com hora automática.
3. Cliente quer sair → operador busca a placa na lista → sistema calcula o valor devido (preço inicial + horas de permanência) → operador confirma a saída.
4. Ao longo do dia, consulta a lista de veículos estacionados e o tempo de permanência de cada um, atualizado em tempo real.

## 4. Brainstorm de funcionalidades

| Prioridade | Funcionalidade |
|---|---|
| Must have | Registrar entrada de veículo (placa + hora automática) |
| Must have | Registrar saída com cálculo automático do valor |
| Must have | Listar veículos estacionados com tempo decorrido |
| Must have | Exibir vagas disponíveis / total |
| Should have | Histórico de saídas (veículos que já saíram) |
| Should have | Persistência real em banco de dados (não em arquivo/memória) |
| Could have | Relatório de faturamento por período |
| Could have | Autenticação de operador |
| Won't have (por ora) | Múltiplos estacionamentos/unidades |

## 5. MVP desta entrega

- **Backend**: API REST em ASP.NET Core (Clean Architecture: Domain, Application, Infrastructure, Api), persistência em SQLite via EF Core, documentação automática (Swagger/OpenAPI).
- **Frontend**: painel web em React + TypeScript, consumindo a API, com atualização em tempo real do tempo estacionado.
- **Empacotamento**: Docker + docker-compose, para subir backend e frontend com um único comando.
- **Testes**: testes automatizados cobrindo as regras de negócio (Domain/Application) e os endpoints principais da API.

## 6. Fora do escopo (próximos passos)

- Autenticação/autorização de operadores
- Múltiplos estacionamentos/unidades
- Relatórios financeiros avançados

## 7. O que existe hoje no repositório

| Versão | Pasta | Descrição |
|---|---|---|
| Desafio original (console) | `src/EstacionamentoDIO.Console` | Entrega original do desafio da DIO, em memória |
| Desktop (Windows Forms) | `src/EstacionamentoDIO.Gui` | Evolução com interface gráfica, persistência em JSON |
| **Sistema completo (API + Web)** | `backend/` e `frontend/` | Versão atual, com arquitetura em camadas, banco de dados e frontend web |
