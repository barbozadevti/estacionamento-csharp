# Sistema de Estacionamento em C#

Desafio de projeto do bootcamp da [DIO](https://www.dio.me/): construção de um sistema de controle de estacionamento em C#/.NET, com duas interfaces sobre a mesma lógica de negócio: console e desktop (Windows Forms).

## Funcionalidades

- **Adicionar veículo**: registra a placa e a hora de entrada (`DateTime.Now`) de um veículo.
- **Remover veículo**: registra a saída, calculando o valor a pagar com base em um preço inicial fixo mais um valor por hora de permanência (hora parcial é cobrada como hora cheia).
- **Listar veículos**: exibe todos os veículos atualmente estacionados, com o horário de entrada (na versão desktop, o tempo estacionado é atualizado ao vivo).
- **Limite de vagas**: o estacionamento tem uma capacidade máxima configurável; ao lotar, novas entradas são recusadas.
- **Persistência em arquivo**: os veículos estacionados são salvos em JSON, então os dados não se perdem ao fechar o programa.

## Tecnologias

- C# / .NET 9
- Windows Forms (interface gráfica desktop)
- xUnit (testes automatizados)

## Estrutura do projeto

```
estacionamento-csharp/
├── EstacionamentoDIO.sln
├── src/
│   ├── EstacionamentoDIO.Core/      # regras de negócio (Estacionamento, Veiculo)
│   ├── EstacionamentoDIO.Console/   # aplicação de console (menu e interação com o usuário)
│   └── EstacionamentoDIO.Gui/       # aplicação desktop com interface gráfica (Windows Forms)
└── tests/
    └── EstacionamentoDIO.Tests/     # testes automatizados (xUnit)
```

## Como executar

Versão console:

```bash
dotnet run --project src/EstacionamentoDIO.Console
```

Versão com interface gráfica (Windows):

```bash
dotnet run --project src/EstacionamentoDIO.Gui
```

## Como rodar os testes

```bash
dotnet test
```
