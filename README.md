# Sistema de Estacionamento em C#

Desafio de projeto do bootcamp da [DIO](https://www.dio.me/): construção de um sistema de controle de estacionamento via console, em C#/.NET.

## Funcionalidades

- **Adicionar veículo**: registra a placa de um veículo que está entrando no estacionamento.
- **Remover veículo**: registra a saída de um veículo, calculando o valor a pagar com base em um preço inicial fixo mais um valor por hora de permanência.
- **Listar veículos**: exibe todos os veículos atualmente estacionados.

## Tecnologias

- C#
- .NET 9 (Console Application)

## Como executar

```bash
dotnet run
```

## Estrutura

- `Program.cs`: menu principal e loop de interação com o usuário.
- `Estacionamento.cs`: classe com a lógica de negócio (adicionar, remover e listar veículos).
