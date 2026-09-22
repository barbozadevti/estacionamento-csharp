using EstacionamentoDIO.Domain.Exceptions;

namespace EstacionamentoDIO.Domain.Entities;

public class Veiculo
{
    public int Id { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public DateTime HoraEntrada { get; private set; }
    public DateTime? HoraSaida { get; private set; }
    public decimal? ValorCobrado { get; private set; }
    public FormaPagamento? FormaPagamento { get; private set; }

    public bool EstaEstacionado => HoraSaida is null;

    private Veiculo()
    {
    }

    public Veiculo(string placa, DateTime horaEntrada)
    {
        if (!Placas.EhValida(placa))
        {
            throw new PlacaInvalidaException(placa);
        }

        Placa = Placas.Normalizar(placa);
        HoraEntrada = horaEntrada;
    }

    public int RegistrarSaida(DateTime horaSaida, decimal precoInicial, decimal precoPorHora, FormaPagamento formaPagamento)
    {
        if (!EstaEstacionado)
        {
            throw new InvalidOperationException("Este veículo já registrou saída.");
        }

        var horas = Math.Max(1, (int)Math.Ceiling((horaSaida - HoraEntrada).TotalHours));

        HoraSaida = horaSaida;
        ValorCobrado = precoInicial + (precoPorHora * horas);
        FormaPagamento = formaPagamento;

        return horas;
    }
}
