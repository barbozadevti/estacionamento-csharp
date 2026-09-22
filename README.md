# Sistema de Estacionamento em C#

Desafio de projeto do bootcamp da [DIO](https://www.dio.me/): construção de um sistema de controle de estacionamento via console, em C#/.NET.

## Funcionalidades

- **Adicionar veículo**: registra a placa e a hora de entrada (`DateTime.Now`) de um veículo.
- **Remover veículo**: registra a saída, calculando o valor a pagar com base em um preço inicial fixo mais um valor por hora de permanência (hora parcial é cobrada como hora cheia).
- **Listar veículos**: exibe todos os veículos atualmente estacionados, com o horário de entrada.
- **Limite de vagas**: o estacionamento tem uma capacidade máxima configurável; ao lotar, novas entradas são recusadas.
- **Persistência em arquivo**: os veículos estacionados são salvos em `estacionamento.json`, então os dados não se perdem ao fechar o programa.

## Tecnologias

- C# / .NET 9
- xUnit (testes automatizados)

## Estrutura do projeto

```
estacionamento-csharp/
├── EstacionamentoDIO.sln
├── src/
│   ├── EstacionamentoDIO.Core/      # regras de negócio (Estacionamento, Veiculo)
│   └── EstacionamentoDIO.Console/   # aplicação de console (menu e interação com o usuário)
└── tests/
    └── EstacionamentoDIO.Tests/     # testes automatizados (xUnit)
```

## Como executar

```bash
dotnet run --project src/EstacionamentoDIO.Console
```

## Como rodar os testes

```bash
dotnet test
```
