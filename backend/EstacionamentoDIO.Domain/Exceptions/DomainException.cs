namespace EstacionamentoDIO.Domain.Exceptions;

public abstract class DomainException : Exception
{
    protected DomainException(string message) : base(message)
    {
    }
}

public class PlacaInvalidaException : DomainException
{
    public PlacaInvalidaException(string? placa)
        : base($"Placa inválida: '{placa}'.")
    {
    }
}

public class VeiculoJaEstacionadoException : DomainException
{
    public VeiculoJaEstacionadoException(string placa)
        : base($"O veículo de placa {placa} já está estacionado.")
    {
    }
}

public class VeiculoNaoEncontradoException : DomainException
{
    public VeiculoNaoEncontradoException(string placa)
        : base($"Nenhum veículo estacionado com a placa {placa}.")
    {
    }
}

public class EstacionamentoLotadoException : DomainException
{
    public EstacionamentoLotadoException()
        : base("Estacionamento lotado. Não há vagas disponíveis.")
    {
    }
}
